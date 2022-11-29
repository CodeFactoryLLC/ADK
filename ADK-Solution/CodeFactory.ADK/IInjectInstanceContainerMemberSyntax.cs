//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************
using System.Threading.Tasks;
using CodeFactory.DotNet.CSharp;

namespace CodeFactory.ADK
{
    /// <summary>
    /// Contract definition for generating syntax from a member model to be added to a target instance based container.
    /// </summary>
    /// <typeparam name="TContainerType">The type of instance based <see cref="CsContainer"/> to be injected into.</typeparam>
    /// <typeparam name="TMemberType">The <see cref="CsMember"/> model used to build the injected syntax.</typeparam>
    public interface IInjectInstanceContainerMemberSyntax<TContainerType, TMemberType> :IInjectContainerMemberSyntax<TContainerType,TMemberType> where TContainerType: CsContainer where TMemberType : CsMember
    {
        /// <summary>
        /// Generates syntax to be injected into the target source.
        /// </summary>
        /// <param name="updateSource">The source and instance container to be updated.</param>
        /// <param name="member">The member model used to generate the syntax.</param>
        /// <param name="targetSecurity">Optional parameter that determines the target security level to set the syntax to.Default is unknown.</param>
        /// <param name="replace">Optional parameter that determines if an existing model should be replaced by the syntax for injection. Default is false.</param>
        /// <returns>The updated source implementation once the syntax is injected or the original source if no changed where made.</returns>
        Task<IUpdateInstanceSource<TContainerType>> InjectSyntaxAsync(IUpdateInstanceSource<TContainerType> updateSource,
            TMemberType member, CsSecurity targetSecurity = CsSecurity.Unknown, bool replace = false);
    }
}
