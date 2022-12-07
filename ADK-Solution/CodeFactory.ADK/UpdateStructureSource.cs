//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************
using System;
using System.Linq;
using System.Threading.Tasks;
using CodeFactory.DotNet.CSharp;
using CodeFactory.Formatting.CSharp;
using CodeFactory.VisualStudio;

namespace CodeFactory.ADK
{
    /// <summary>
    /// Tracks updates for a target structure and the source for that structure.
    /// </summary>
    public class UpdateStructureSource:UpdateSourceBase<CsStructure>,IUpdateInstanceSource<CsStructure>
    {
        /// <summary>
        /// Creates an instance of <see cref="UpdateClassSource"/>
        /// </summary>
        /// <param name="source">The source code file to update.</param>
        /// <param name="container">The structure model that is to be updated.</param>
        /// <param name="vsActions">The CodeFactory actions API.</param>
        /// <param name="namespaceManager">The namespace manager to use with the update source implementation.</param>
        public UpdateStructureSource(CsSource source, CsStructure container, IVsActions vsActions, NamespaceManager namespaceManager) : base(source, container, vsActions, namespaceManager)
        {
            //Intentionally blank
        }

        /// <summary>
        /// Adds the provided syntax before the field definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task FieldsAddBeforeAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = Source ?? throw new ArgumentNullException(nameof(Source));
            var container = Container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.Fields.Any(f => f.ModelSourceFile == sourceDoc & f.LoadedFromSource))
            {
                var fieldData  = container.Fields.First(f => f.ModelSourceFile == sourceDoc & f.LoadedFromSource);

                var updatedSource = await fieldData.AddBeforeAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<CsStructure>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.ContainerAddToBeginningAsync(syntax);
            }
        }

        /// <summary>
        /// Adds the provided syntax after the field definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task FieldsAddAfterAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = Source ?? throw new ArgumentNullException(nameof(Source));
            var container = Container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.Fields.Any(f => f.ModelSourceFile == sourceDoc & f.LoadedFromSource))
            {
                var fieldData  = container.Fields.Last(f => f.ModelSourceFile == sourceDoc & f.LoadedFromSource);

                var updatedSource = await fieldData.AddAfterAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<CsStructure>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.ContainerAddToBeginningAsync(syntax);
            }
        }

        /// <summary>
        /// Add the provided syntax before the constructor definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task ConstructorsAddBeforeAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = Source ?? throw new ArgumentNullException(nameof(Source));
            var container = Container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.Constructors.Any(c => c.ModelSourceFile == sourceDoc & c.LoadedFromSource))
            {
                var constData  = container.Constructors.First(c => c.ModelSourceFile == sourceDoc & c.LoadedFromSource);

                var updatedSource = await constData.AddBeforeAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<CsStructure>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.FieldsAddAfterAsync(syntax);
            }
        }

        /// <summary>
        /// Add the provided syntax after the constructor definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public async Task ConstructorsAddAfterAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = Source ?? throw new ArgumentNullException(nameof(Source));
            var container = Container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.Constructors.Any(c => c.ModelSourceFile == sourceDoc & c.LoadedFromSource))
            {
                var constData  = container.Constructors.Last(c => c.ModelSourceFile == sourceDoc & c.LoadedFromSource);

                var updatedSource = await constData.AddAfterAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<CsStructure>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.FieldsAddAfterAsync(syntax);
            }
        }

        /// <summary>
        /// Add the provided syntax before the property definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public new async Task PropertiesAddBeforeAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = Source ?? throw new ArgumentNullException(nameof(Source));
            var container = Container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.Properties.Any(p => p.ModelSourceFile == sourceDoc & p.LoadedFromSource))
            {
                var propertyData  = container.Properties.First(p => p.ModelSourceFile == sourceDoc & p.LoadedFromSource);

                var updatedSource = await propertyData.AddBeforeAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<CsStructure>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.ConstructorsAddAfterAsync(syntax);
            }
        }

        /// <summary>
        /// Add the provided syntax after the property definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        public new async Task PropertiesAddAfterAsync(string syntax)
        {
            if(string.IsNullOrEmpty(syntax)) return;
            var source = Source ?? throw new ArgumentNullException(nameof(Source));
            var container = Container ?? throw new ArgumentNullException(nameof(Container));

            var sourceDoc = source.SourceDocument;

            if (container.Properties.Any(p => p.ModelSourceFile == sourceDoc & p.LoadedFromSource))
            {
                var propertyData  = container.Properties.Last(p => p.ModelSourceFile == sourceDoc & p.LoadedFromSource);

                var updatedSource = await propertyData.AddAfterAsync(syntax);

                if (updatedSource == null) throw new ArgumentNullException(nameof(Source));

                var updatedContainer = updatedSource.GetModel<CsStructure>(ContainerPath);

                UpdateSources(updatedSource,updatedContainer);
            }
            else
            {
                await this.ConstructorsAddAfterAsync(syntax);
            }
        }
    }
}
