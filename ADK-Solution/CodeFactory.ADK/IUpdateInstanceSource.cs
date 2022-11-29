//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************
using System;
using System.Threading.Tasks;
using CodeFactory.DotNet.CSharp;

namespace CodeFactory.ADK
{
    /// <summary>
    /// Contract that is used to track the source code and target instance container that is being updated.
    /// </summary>
    /// <typeparam name="TContainerType">Target type of the container that is being updated.</typeparam>
    public interface IUpdateInstanceSource<TContainerType>:IUpdateSource<TContainerType> where TContainerType : CsContainer
    {
        /// <summary>
        /// Adds the provided syntax before the field definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        Task FieldsAddBeforeAsync(string syntax);

        /// <summary>
        /// Adds the provided syntax after the field definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        Task FieldsAddAfterAsync(string syntax);

        /// <summary>
        /// Add the provided syntax before the constructor definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        Task ConstructorsAddBeforeAsync(string syntax);

        /// <summary>
        /// Add the provided syntax after the constructor definitions.
        /// </summary>
        /// <param name="syntax">Target syntax to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the source or the container is null after updating.</exception>
        Task ConstructorsAddAfterAsync(string syntax);

    }
}
