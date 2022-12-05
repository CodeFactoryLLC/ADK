//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************
using CodeFactory.DotNet.CSharp;
using System;
using System.Linq;
using System.Threading.Tasks;
using CodeFactory.Formatting.CSharp;
using CodeFactory.VisualStudio;

namespace CodeFactory.ADK
{
    /// <summary>
    /// Base class implementation of the <see cref="IUpdateSource{TContainerType}"/> contract.
    /// </summary>
    /// <typeparam name="TContainerType">Target <see cref="CsContainer"/> type.</typeparam>
    public abstract class UpdateSourceBase<TContainerType>:IUpdateSource<TContainerType> where TContainerType : CsContainer
    {
        //Backing fields for properties.
        private CsSource _source;
        private TContainerType _container;
        private readonly IVsActions _vsActions;
        private NamespaceManager _namespaceManager;

        //Lookup path used for loading the container from the source. 
        protected readonly string ContainerPath;

        protected UpdateSourceBase(CsSource source, TContainerType container, IVsActions vsActions, NamespaceManager namespaceManager)
        {
            _source = source;
            _container = container;

            if (_container != null) ContainerPath = container.LookupPath;

            _vsActions = vsActions;
            _namespaceManager = namespaceManager;
        }

        /// <summary>
        /// Target source that is being updated.
        /// </summary>
        public CsSource Source => _source;

        /// <summary>
        /// Target container being updated.
        /// </summary>
        public TContainerType Container => _container;

        /// <summary>
        /// The code factory actions for visual studio to be used with updates to the source.
        /// </summary>
        public IVsActions VsActions => _vsActions;

        /// <summary>
        /// The namespace manager that is used for updating source.
        /// </summary>
        public NamespaceManager NamespaceManager => _namespaceManager;

        /// <summary>
        /// Refreshes the current version of the update sources.
        /// </summary>
        /// <param name="source">The updated <see cref="CsSource"/>.</param>
        /// <param name="container">The updates hosting <see cref="CsContainer"/> type.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null.</exception>
        public void UpdateSources(CsSource source, TContainerType container)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <summary>
        /// Refreshes the current version of the namespace manager for the sources.
        /// </summary>
        /// <param name="namespaceManager">Updated namespace to register</param>
        /// <exception cref="ArgumentNullException">Thrown if the namespace manager is null.</exception>
        public void UpdateNamespaceManager(NamespaceManager namespaceManager)
        {
            _namespaceManager = namespaceManager ?? throw new ArgumentNullException(nameof(namespaceManager));
        }

        /// <summary>
        /// Loads a new instance of a <see cref="IUpdateSource{TContainerType}.NamespaceManager"/> from the current source and assigns it to the <see cref="IUpdateSource{TContainerType}.NamespaceManager"/> property.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if either the source or container is null.</exception>
        public void LoadNamespaceManager()
        {
            if (_source == null) throw new ArgumentNullException(nameof(Source));

            if(!_source.NamespaceReferences.Any()) return;

            if(_container == null) throw new ArgumentNullException(nameof(Container));

            var updatedNamespaceManager = new NamespaceManager(_source.NamespaceReferences, _container.Namespace);

            UpdateNamespaceManager(updatedNamespaceManager);
        }

        /// <summary>
        /// Creates a new using statement in the source if the using statement does not exist. It will also reload the namespace manager and update it.
        /// </summary>
        /// <param name="nameSpace">Namespace to add to the source file.</param>
        /// <param name="alias">Optional parameter to assign a alias to the using statement.</param>
        /// <exception cref="ArgumentNullException">Thrown if the source is null.</exception>
        public async Task UsingStatementAddAsync(string nameSpace, string alias = null)
        {
            if (_source == null) throw new ArgumentNullException(nameof(Source));

            if(string.IsNullOrEmpty(nameSpace)) return;

            var updatedSource = await Source.AddUsingStatementAsync(nameSpace, alias);

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);

            LoadNamespaceManager();
        }

        /// <summary>
        /// Adds the provided syntax to the beginning of the source file.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task SourceAddToBeginningAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));

            var updatedSource = await source.AddToBeginningAsync(syntax);

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Adds the provided syntax to the end of the source file.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task SourceAddToEndAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));

            var updatedSource = await source.AddToEndAsync(syntax);

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Adds the provided syntax before the containers definition.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task ContainerAddBeforeAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;

            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var updatedSource = await container.AddBeforeAsync(syntax);

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Adds the provided syntax after containers definition.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task ContainerAddAfterAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;

            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var updatedSource = await container.AddAfterAsync(syntax);

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Adds the provided syntax to the beginning of the containers definition.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task ContainerAddToBeginningAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var updatedSource = await container.AddToBeginningAsync(syntax);

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Adds the provided syntax to the end of the containers definition.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task ContainerAddToEndAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var updatedSource = await container.AddToEndAsync(syntax);

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Adds the provided syntax before the first using statement definition.
        /// </summary>
        /// <param name="syntax">Target syntax to be added</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task UsingStatementsAddBeforeAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));

            CsSource updatedSource = null;

            var sourceDoc = source.SourceDocument;
            if (source.NamespaceReferences.Any(n => n.SourceDocument == sourceDoc & n.LoadedFromSource))
            {
                var usingStatement = source.NamespaceReferences.First(n => n.SourceDocument == sourceDoc & n.LoadedFromSource );

                updatedSource = await usingStatement.AddBeforeAsync(syntax);
            }
            else
            {
                updatedSource =  await source.AddToBeginningAsync(syntax);
            }
            
            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Adds the provided syntax before the first using statement definition.
        /// </summary>
        /// <param name="syntax">Target syntax to be added</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task UsingStatementsAddAfterAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));

            CsSource updatedSource = null;

            var sourceDoc = source.SourceDocument;

            if (source.NamespaceReferences.Any(n => n.SourceDocument == sourceDoc & n.LoadedFromSource))
            {
                var usingStatement = source.NamespaceReferences.Last(n => n.SourceDocument == sourceDoc & n.LoadedFromSource);

                updatedSource = await usingStatement.AddAfterAsync(syntax);
            }
            else
            {
                updatedSource =  await source.AddToBeginningAsync(syntax);
            }
            
            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Add the provided syntax before the property definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task PropertiesAddBeforeAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.Properties.Any(p => p.ModelSourceFile == sourceDoc & p.LoadedFromSource))
            {
                var propertyData  = container.Properties.First(p => p.ModelSourceFile == sourceDoc & p.LoadedFromSource);

                var updatedSource = await propertyData.AddBeforeAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.ContainerAddToBeginningAsync(syntax);
            }
            
        }

        /// <summary>
        /// Add the provided syntax after the property definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task PropertiesAddAfterAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.Properties.Any(p => p.ModelSourceFile == sourceDoc & p.LoadedFromSource))
            {
                var propertyData  = container.Properties.Last(p => p.ModelSourceFile == sourceDoc & p.LoadedFromSource);

                var updatedSource = await propertyData.AddAfterAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.ContainerAddToBeginningAsync(syntax);
            }
        }

        /// <summary>
        /// Add the provided syntax before the event definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task EventsAddBeforeAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.Events.Any(e => e.ModelSourceFile == sourceDoc & e.LoadedFromSource))
            {
                var eventData  = container.Events.First(e => e.ModelSourceFile == sourceDoc & e.LoadedFromSource);

                var updatedSource = await eventData.AddBeforeAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.PropertiesAddAfterAsync(syntax);
            }
        }

        /// <summary>
        /// Add the provided syntax after the event definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task EventsAddAfterAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.Events.Any(e => e.ModelSourceFile == sourceDoc & e.LoadedFromSource))
            {
                var eventData  = container.Events.Last(e => e.ModelSourceFile == sourceDoc & e.LoadedFromSource);

                var updatedSource = await eventData.AddAfterAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.PropertiesAddAfterAsync(syntax);
            }
        }

        /// <summary>
        /// Add the provided syntax before the method definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task MethodsAddBeforeAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.Methods.Any(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource))
            {
                var methodData  = container.Methods.First(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource);

                var updatedSource = await methodData.AddBeforeAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.EventsAddAfterAsync(syntax);
            }
        }

        /// <summary>
        /// Add the provided syntax after the method definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task MethodsAddAfterAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.Methods.Any(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource))
            {
                var methodData  = container.Methods.Last(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource);

                var updatedSource = await methodData.AddAfterAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.EventsAddAfterAsync(syntax);
            }
        }

        /// <summary>
        /// Add the syntax before the target member.
        /// </summary>
        /// <param name="member">Target member.</param>
        /// <param name="syntax">The syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task MemberAddBeforeAsync(CsMember member, string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;

            if (member == null) throw new ArgumentNullException(nameof(member));

            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            CsSource updatedSource = null;

            updatedSource = await member.AddBeforeAsync(syntax);

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
           
        }

        /// <summary>
        /// Add the syntax after the target member.
        /// </summary>
        /// <param name="member">Target member.</param>
        /// <param name="syntax">The syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task MemberAddAfterAsync(CsMember member, string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;

            if (member == null) throw new ArgumentNullException(nameof(member));

            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            CsSource updatedSource = null;

            updatedSource = await member.AddAfterAsync(syntax);

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Syntax replaces the target member.
        /// </summary>
        /// <param name="member">Target member.</param>
        /// <param name="syntax">The syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task MemberReplaceAsync(CsMember member, string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;

            if (member == null) throw new ArgumentNullException(nameof(member));

            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            CsSource updatedSource = null;

            updatedSource = await member.ReplaceAsync(syntax);

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Removes the target member.
        /// </summary>
        /// <param name="member">Target member.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task MemberRemoveAsync(CsMember member)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));

            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            CsSource updatedSource = null;

            updatedSource = await member.DeleteAsync();

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Add the provided syntax before the nested enumeration definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task NestedEnumAddBeforeAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.NestedEnums.Any(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource))
            {
                var enumData  = container.NestedEnums.First(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource);

                var updatedSource = await enumData.AddBeforeAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.MethodsAddAfterAsync(syntax);
            }
        }

        /// <summary>
        /// Add the provided syntax after the nested enumeration definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task NestedEnumAddAfterAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.NestedEnums.Any(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource))
            {
                var enumData  = container.NestedEnums.Last(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource);

                var updatedSource = await enumData.AddAfterAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.MethodsAddAfterAsync(syntax);
            }
        }

        /// <summary>
        /// Removes the nested enumeration.
        /// </summary>
        /// <param name="nested">The target nested enumeration.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task NestedEnumRemoveAsync(CsEnum nested)
        {
            if (nested == null) throw new ArgumentNullException(nameof(nested));

            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            CsSource updatedSource = null;

            updatedSource = await nested.DeleteAsync();

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Replaces the nested enumeration with the provided syntax
        /// </summary>
        /// <param name="nested">The target nested enumeration.</param>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task NestedEnumReplaceAsync(CsEnum nested, string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            if (nested == null) throw new ArgumentNullException(nameof(nested));

            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            CsSource updatedSource = null;

            updatedSource = await nested.ReplaceAsync(syntax);

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Add the provided syntax before the nested interface definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task NestedInterfaceAddBeforeAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.NestedInterfaces.Any(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource))
            {
                var interfaceData  = container.NestedInterfaces.First(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource);

                var updatedSource = await interfaceData.AddBeforeAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.NestedEnumAddAfterAsync(syntax);
            }
        }

        /// <summary>
        /// Add the provided syntax after the nested interface definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task NestedInterfaceAddAfterAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.NestedInterfaces.Any(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource))
            {
                var interfaceData  = container.NestedInterfaces.Last(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource);

                var updatedSource = await interfaceData.AddAfterAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.NestedEnumAddAfterAsync(syntax);
            }
        }

        /// <summary>
        /// Removes the nested interface.
        /// </summary>
        /// <param name="nested">The target nested interface.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task NestedInterfaceRemoveAsync(CsInterface nested)
        {
            if (nested == null) throw new ArgumentNullException(nameof(nested));

            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            CsSource updatedSource = null;

            updatedSource = await nested.DeleteAsync();

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Replaces the nested interface with the provided syntax
        /// </summary>
        /// <param name="nested">The target nested interface.</param>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task NestedInterfaceReplaceAsync(CsInterface nested, string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            if (nested == null) throw new ArgumentNullException(nameof(nested));

            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            CsSource updatedSource = null;

            updatedSource = await nested.ReplaceAsync(syntax);

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Add the provided syntax before the nested structures definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task NestedStructuresAddBeforeAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.NestedStructures.Any(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource))
            {
                var structData  = container.NestedStructures.First(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource);

                var updatedSource = await structData.AddBeforeAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.NestedInterfaceAddAfterAsync(syntax);
            }
        }

        /// <summary>
        /// Add the provided syntax after the nested structures definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task NestedStructuresAddAfterAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.NestedStructures.Any(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource))
            {
                var structData  = container.NestedStructures.Last(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource);

                var updatedSource = await structData.AddAfterAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.NestedInterfaceAddAfterAsync(syntax);
            }
        }

        /// <summary>
        /// Removes the nested structure.
        /// </summary>
        /// <param name="nested">The target nested structure.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task NestedStructureRemoveAsync(CsStructure nested)
        {
            if (nested == null) throw new ArgumentNullException(nameof(nested));

            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            CsSource updatedSource = null;

            updatedSource = await nested.DeleteAsync();

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Replaces the nested structure with the provided syntax
        /// </summary>
        /// <param name="nested">The target nested structure.</param>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task NestedStructureReplaceAsync(CsStructure nested, string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            if (nested == null) throw new ArgumentNullException(nameof(nested));

            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            CsSource updatedSource = null;

            updatedSource = await nested.ReplaceAsync(syntax);

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Add the provided syntax before the nested classes definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task NestedClassesAddBeforeAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.NestedClasses.Any(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource))
            {
                var classData  = container.NestedClasses.First(m => m.ModelSourceFile == sourceDoc & m.LoadedFromSource);

                var updatedSource = await classData.AddBeforeAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.NestedStructuresAddAfterAsync(syntax);
            }
        }

        /// <summary>
        /// Add the provided syntax after the nested classes definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task NestedClassesAddAfterAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.NestedClasses.Any(m => m.ModelSourceFile == sourceDoc))
            {
                var classData  = container.NestedClasses.Last(m => m.ModelSourceFile == sourceDoc);

                var updatedSource = await classData.AddAfterAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.NestedStructuresAddAfterAsync(syntax);
            }
        }

        /// <summary>
        /// Removes the nested class.
        /// </summary>
        /// <param name="nested">The target nested class.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task NestedClassesRemoveAsync(CsClass nested)
        {
            if (nested == null) throw new ArgumentNullException(nameof(nested));

            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            CsSource updatedSource = null;

            updatedSource = await nested.DeleteAsync();

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }

        /// <summary>
        /// Replaces the nested class with the provided syntax
        /// </summary>
        /// <param name="nested">The target nested class.</param>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task NestedClassesReplaceAsync(CsClass nested, string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            if (nested == null) throw new ArgumentNullException(nameof(nested));

            var source = _source ?? throw new ArgumentNullException(nameof(Source));
            var container = _container ?? throw new ArgumentNullException(nameof(Container));

            CsSource updatedSource = null;

            updatedSource = await nested.ReplaceAsync(syntax);

            if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

            var updatedContainer = updatedSource.GetModel<TContainerType>(ContainerPath);

            UpdateSources(updatedSource,updatedContainer);
        }
    }
}
