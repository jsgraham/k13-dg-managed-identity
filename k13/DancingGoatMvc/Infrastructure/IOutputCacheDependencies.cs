﻿using System;
using System.Collections.Generic;
using CMS.DataEngine;
using CMS.DocumentEngine;

namespace DancingGoat.Infrastructure
{
    /// <summary>
    /// Represents a contract for objects that create a minimum set of ASP.NET output cache dependencies for views that contain data from pages or info objects.
    /// </summary>
    public interface IOutputCacheDependencies
    {
        /// <summary>
        /// Adds a minimum set of ASP.NET output cache dependencies for a view that contains data from pages of the specified runtime type.
        /// When any page of the specified runtime type is created, updated or deleted, the corresponding output cache item is invalidated.
        /// </summary>
        /// <param name="pages">Pages used to create validation callback to invalidate cache based on <see cref="TreeNode.DocumentPublishTo"/>.</param>
        /// <typeparam name="T">Runtime type that represents pages, i.e. it is derived from the <see cref="TreeNode"/> class.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="pages"/> is <c>null</c>.</exception>
        void AddDependencyOnPages<T>(IEnumerable<T> pages) where T : TreeNode, new();


        /// <summary>
        /// Adds a minimum set of ASP.NET output cache dependencies for a view that contains data from page of the specified runtime type.
        /// When specified page of the specified runtime type is created, updated or deleted, the corresponding output cache item is invalidated.
        /// </summary>
        /// <param name="page">Page used for dependency cache key.</param>
        /// <typeparam name="T">Runtime type that represents pages, i.e. it is derived from the <see cref="TreeNode"/> class.</typeparam>
        void AddDependencyOnPage<T>(T page) where T : TreeNode, new();


        /// <summary>
        /// Adds a minimum set of ASP.NET output cache dependencies for a view that contains data from page attachment.
        /// When specified attachment is created, updated or deleted, the corresponding output cache item is invalidated.
        /// </summary>
        /// <param name="attachmentGuid">Attachment guid used for dependency cache key.</param>
        void AddDependencyOnPageAttachmnent(Guid attachmentGuid);


        /// <summary>
        /// Adds a minimum set of ASP.NET output cache dependencies for a view that contains data from info objects of the specified runtime type.
        /// When any info object of the specified runtime type is created, updated or deleted, the corresponding output cache item is invalidated.
        /// </summary>
        /// <typeparam name="T">Runtime type that represents info objects, i.e. it is derived from the <see cref="AbstractInfo{TInfo}"/> class.</typeparam>
        void AddDependencyOnInfoObjects<T>() where T : AbstractInfo<T>, new();


        /// <summary>
        /// Adds a minimum set of ASP.NET output cache dependencies for a view that contains data from info object of the specified runtime type.
        /// When info object of the specified runtime type is created, updated or deleted, the corresponding output cache item is invalidated.
        /// </summary>
        /// <param name="infoGuid">Info object guid used for dependency cache key.</param>
        /// <typeparam name="T">Runtime type that represents info objects, i.e. it is derived from the <see cref="AbstractInfo{TInfo}"/> class.</typeparam>
        void AddDependencyOnInfoObject<T>(Guid infoGuid) where T : AbstractInfo<T>, new();
    }
}