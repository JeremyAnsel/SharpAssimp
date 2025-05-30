﻿/*
* Copyright (c) 2012-2020 AssimpNet - Nicholas Woodfield
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using SharpAssimp.Unmanaged;

namespace SharpAssimp
{
    /// <summary>
    /// Represents a completely imported model or scene. Everything that was imported from the given file can be
    /// accessed from here. Once the scene is loaded from unmanaged memory, it resides solely in managed memory
    /// and Assimp's read only copy is released.
    /// </summary>
    public sealed class Scene : IMarshalable<Scene, AiScene>
    {
        private SceneFlags m_flags;
        private Node? m_rootNode;
        private readonly List<Mesh> m_meshes;
        private readonly List<Light> m_lights;
        private readonly List<Camera> m_cameras;
        private readonly List<EmbeddedTexture?> m_textures;
        private readonly List<Animation> m_animations;
        private readonly List<Material> m_materials;
        private Metadata m_metadata;
        private string m_name;

        /// <summary>
        /// Gets or sets the state of the imported scene. By default no flags are set, but
        /// issues can arise if the flag is set to incomplete.
        /// </summary>
        public SceneFlags SceneFlags
        {
            get => m_flags;
            set => m_flags = value;
        }

        /// <summary>
        /// Gets or sets the root node of the scene graph. There will always be at least the root node
        /// if the import was successful and no special flags have been set. Presence of further nodes
        /// depends on the format and content of the imported file.
        /// </summary>
        public Node? RootNode
        {
            get => m_rootNode;
            set => m_rootNode = value;
        }

        /// <summary>
        /// Gets if the scene contains meshes. Unless if no special scene flags are set
        /// this should always be true.
        /// </summary>
        public bool HasMeshes => m_meshes.Count > 0;

        /// <summary>
        /// Gets the number of meshes in the scene.
        /// </summary>
        public int MeshCount => m_meshes.Count;

        /// <summary>
        /// Gets the meshes contained in the scene, if any.
        /// </summary>
        public List<Mesh> Meshes => m_meshes;


        /// <summary>
        /// Gets if the scene contains any lights.
        /// </summary>
        public bool HasLights => m_lights.Count > 0;

        /// <summary>
        /// Gets the number of lights in the scene.
        /// </summary>
        public int LightCount => m_lights.Count;

        /// <summary>
        /// Gets the lights in the scene, if any.
        /// </summary>
        public List<Light> Lights => m_lights;

        /// <summary>
        /// Gets if the scene contains any cameras.
        /// </summary>
        public bool HasCameras => m_cameras.Count > 0;

        /// <summary>
        /// Gets the number of cameras in the scene.
        /// </summary>
        public int CameraCount => m_cameras.Count;

        /// <summary>
        /// Gets the cameras in the scene, if any.
        /// </summary>
        public List<Camera> Cameras => m_cameras;

        /// <summary>
        /// Gets if the scene contains embedded textures.
        /// </summary>
        public bool HasTextures => m_textures.Count > 0;

        /// <summary>
        /// Gets the number of embedded textures in the scene.
        /// </summary>
        public int TextureCount => m_textures.Count;

        /// <summary>
        /// Gets the embedded textures in the scene, if any.
        /// </summary>
        public List<EmbeddedTexture?> Textures => m_textures;

        /// <summary>
        /// Gets if the scene contains any animations.
        /// </summary>
        public bool HasAnimations => m_animations.Count > 0;

        /// <summary>
        /// Gets the number of animations in the scene.
        /// </summary>
        public int AnimationCount => m_animations.Count;

        /// <summary>
        /// Gets the animations in the scene, if any.
        /// </summary>
        public List<Animation> Animations => m_animations;

        /// <summary>
        /// Gets if the scene contains any materials. There should always be at least the
        /// default Assimp material if no materials were loaded.
        /// </summary>
        public bool HasMaterials => m_materials.Count > 0;

        /// <summary>
        /// Gets the number of materials in the scene. There should always be at least the
        /// default Assimp material if no materials were loaded.
        /// </summary>
        public int MaterialCount => m_materials.Count;

        /// <summary>
        /// Gets the materials in the scene.
        /// </summary>
        public List<Material> Materials => m_materials;

        /// <summary>
        /// Gets the metadata of the scene. This data contains global metadata which belongs to the scene like 
        /// unit-conversions, versions, vendors or other model-specific data. This can be used to store format-specific metadata as well.
        /// </summary>
        public Metadata Metadata => m_metadata;

        /// <summary>
        /// Gets or sets the name of the scene.
        /// </summary>
        public string Name
        {
            get => m_name;
            set => m_name = value;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Scene"/> class.
        /// </summary>
        public Scene()
        {
            m_flags = SceneFlags.None;
            m_rootNode = null;
            m_meshes = [];
            m_lights = [];
            m_cameras = [];
            m_textures = [];
            m_animations = [];
            m_materials = [];
            m_metadata = [];
            m_name = string.Empty;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Scene"/> class.
        /// </summary>
        /// <param name="name">Name of the scene</param>
        public Scene(string name)
            : this()
        {
            m_name = name;
        }

        /// <summary>
        /// Clears the scene of all components.
        /// </summary>
        public void Clear()
        {
            m_rootNode = null;
            m_meshes.Clear();
            m_lights.Clear();
            m_cameras.Clear();
            m_textures.Clear();
            m_animations.Clear();
            m_materials.Clear();
            m_metadata.Clear();
        }

        /// <summary>
        /// Gets an embedded texture by a string. The string may be a texture ID in the format of "*1" or is the
        /// file name of the texture.
        /// </summary>
        /// <param name="fileName">Texture ID or original file name.</param>
        /// <returns>Embedded texture or null if it could not be found.</returns>
        public EmbeddedTexture? GetEmbeddedTexture(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            //Lookup using texture ID (if referenced like: "*1", "*2", etc)
            if (fileName.StartsWith('*'))
            {
                string indexStr = fileName[1..];
                if (!int.TryParse(indexStr, out int index) || index < 0 || index >= m_textures.Count)
                    return null;

                return m_textures[index];
            }

            //Lookup using filename
            string shortFileName = Path.GetFileName(fileName);
            foreach (EmbeddedTexture? tex in m_textures)
            {
                if (tex == null)
                    continue;

                string otherFilename = Path.GetFileName(tex.Filename);
                if (string.Equals(shortFileName, otherFilename, StringComparison.Ordinal))
                    return tex;
            }

            return null;
        }

        /// <summary>
        /// Marshals a managed scene to unmanaged memory. The unmanaged memory must be freed with a call to
        /// <see cref="FreeUnmanagedScene"/>, the memory is owned by AssimpNet and cannot be freed by the native library.
        /// </summary>
        /// <param name="scene">Scene data</param>
        /// <returns>Unmanaged scene or NULL if the scene is null.</returns>
        public static IntPtr ToUnmanagedScene(Scene scene)
        {
            if (scene == null)
                return IntPtr.Zero;

            return MemoryHelper.ToNativePointer<Scene, AiScene>(scene);
        }

        /// <summary>
        /// Marshals an unmanaged scene to managed memory. This does not free the unmanaged memory.
        /// </summary>
        /// <param name="scenePtr">The unmanaged scene data</param>
        /// <returns>The managed scene, or null if the pointer is NULL</returns>
        public static Scene? FromUnmanagedScene(IntPtr scenePtr)
        {
            if (scenePtr == IntPtr.Zero)
                return null;

            return MemoryHelper.FromNativePointer<Scene, AiScene>(scenePtr);
        }

        /// <summary>
        /// Frees unmanaged memory allocated -ONLY- in <see cref="ToUnmanagedScene"/>. To free an unmanaged scene allocated by the unmanaged Assimp library,
        /// call the appropiate <see cref="AssimpLibrary.ReleaseImport"/> function.
        /// </summary>
        /// <param name="scenePtr">Pointer to unmanaged scene data.</param>
        public static void FreeUnmanagedScene(IntPtr scenePtr)
        {
            if (scenePtr == IntPtr.Zero)
                return;

            FreeNative(scenePtr, true);
        }

        #region IMarshalable Implementation

        /// <summary>
        /// Gets if the native value type is blittable (that is, does not require marshaling by the runtime, e.g. has MarshalAs attributes).
        /// </summary>
        bool IMarshalable<Scene, AiScene>.IsNativeBlittable => true;

        /// <summary>
        /// Writes the managed data to the native value.
        /// </summary>
        /// <param name="thisPtr">Optional pointer to the memory that will hold the native value.</param>
        /// <param name="nativeValue">Output native value</param>
        void IMarshalable<Scene, AiScene>.ToNative(IntPtr thisPtr, out AiScene nativeValue)
        {
            nativeValue.Flags = m_flags;
            nativeValue.Materials = IntPtr.Zero;
            nativeValue.RootNode = IntPtr.Zero;
            nativeValue.Meshes = IntPtr.Zero;
            nativeValue.Lights = IntPtr.Zero;
            nativeValue.Cameras = IntPtr.Zero;
            nativeValue.Textures = IntPtr.Zero;
            nativeValue.Animations = IntPtr.Zero;
            nativeValue.Metadata = IntPtr.Zero;
            nativeValue.Name = new AiString(m_name);
            nativeValue.Skeletons = IntPtr.Zero;
            nativeValue.Private = IntPtr.Zero;

            nativeValue.NumMaterials = (uint)MaterialCount;
            nativeValue.NumMeshes = (uint)MeshCount;
            nativeValue.NumLights = (uint)LightCount;
            nativeValue.NumCameras = (uint)CameraCount;
            nativeValue.NumTextures = (uint)TextureCount;
            nativeValue.NumAnimations = (uint)AnimationCount;
            nativeValue.NumSkeletons = 0;

            //Write materials
            if (nativeValue.NumMaterials > 0)
                nativeValue.Materials = MemoryHelper.ToNativeArray<Material, AiMaterial>(m_materials, true);

            //Write scenegraph
            if (m_rootNode != null)
                nativeValue.RootNode = MemoryHelper.ToNativePointer<Node, AiNode>(m_rootNode);

            //Write meshes
            if (nativeValue.NumMeshes > 0)
                nativeValue.Meshes = MemoryHelper.ToNativeArray<Mesh, AiMesh>(m_meshes, true);

            //Write lights
            if (nativeValue.NumLights > 0)
                nativeValue.Lights = MemoryHelper.ToNativeArray<Light, AiLight>(m_lights, true);

            //Write cameras
            if (nativeValue.NumCameras > 0)
                nativeValue.Cameras = MemoryHelper.ToNativeArray<Camera, AiCamera>(m_cameras, true);

            //Write textures
            if (nativeValue.NumTextures > 0)
                nativeValue.Textures = MemoryHelper.ToNativeArray<EmbeddedTexture, AiTexture>(m_textures!, true);

            //Write animations
            if (nativeValue.NumAnimations > 0)
                nativeValue.Animations = MemoryHelper.ToNativeArray<Animation, AiAnimation>(m_animations, true);

            //Write metadata
            if (m_metadata.Count > 0)
                nativeValue.Metadata = MemoryHelper.ToNativePointer<Metadata, AiMetadata>(m_metadata);
        }

        /// <summary>
        /// Reads the unmanaged data from the native value.
        /// </summary>
        /// <param name="nativeValue">Input native value</param>
        void IMarshalable<Scene, AiScene>.FromNative(in AiScene nativeValue)
        {
            Clear();

            m_flags = nativeValue.Flags;
            m_name = AiString.GetString(nativeValue.Name); //Avoid struct copy

            //Read materials
            if (nativeValue.NumMaterials > 0 && nativeValue.Materials != IntPtr.Zero)
                m_materials.AddRange(MemoryHelper.FromNativeArray<Material, AiMaterial>(nativeValue.Materials, (int)nativeValue.NumMaterials, true));

            //Read scenegraph
            if (nativeValue.RootNode != IntPtr.Zero)
                m_rootNode = MemoryHelper.FromNativePointer<Node, AiNode>(nativeValue.RootNode);

            //Read meshes
            if (nativeValue.NumMeshes > 0 && nativeValue.Meshes != IntPtr.Zero)
                m_meshes.AddRange(MemoryHelper.FromNativeArray<Mesh, AiMesh>(nativeValue.Meshes, (int)nativeValue.NumMeshes, true));

            //Read lights
            if (nativeValue.NumLights > 0 && nativeValue.Lights != IntPtr.Zero)
                m_lights.AddRange(MemoryHelper.FromNativeArray<Light, AiLight>(nativeValue.Lights, (int)nativeValue.NumLights, true));

            //Read cameras
            if (nativeValue.NumCameras > 0 && nativeValue.Cameras != IntPtr.Zero)
                m_cameras.AddRange(MemoryHelper.FromNativeArray<Camera, AiCamera>(nativeValue.Cameras, (int)nativeValue.NumCameras, true));

            //Read textures
            if (nativeValue.NumTextures > 0 && nativeValue.Textures != IntPtr.Zero)
                m_textures.AddRange(MemoryHelper.FromNativeArray<EmbeddedTexture, AiTexture>(nativeValue.Textures, (int)nativeValue.NumTextures, true));

            //Read animations
            if (nativeValue.NumAnimations > 0 && nativeValue.Animations != IntPtr.Zero)
                m_animations.AddRange(MemoryHelper.FromNativeArray<Animation, AiAnimation>(nativeValue.Animations, (int)nativeValue.NumAnimations, true));

            //Read metadata
            if (nativeValue.Metadata != IntPtr.Zero)
            {
                m_metadata = MemoryHelper.FromNativePointer<Metadata, AiMetadata>(nativeValue.Metadata)!;

                // Make sure we never have a null instance
                m_metadata ??= [];
            }
        }


        /// <summary>
        /// Frees unmanaged memory created by <see cref="IMarshalable{Scene, AiScene}.ToNative"/>.
        /// </summary>
        /// <param name="nativeValue">Native value to free</param>
        /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise.</param>
        public static void FreeNative(IntPtr nativeValue, bool freeNative)
        {
            if (nativeValue == IntPtr.Zero)
                return;

            AiScene aiScene = MemoryHelper.Read<AiScene>(nativeValue);

            if (aiScene.NumMaterials > 0 && aiScene.Materials != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiMaterial>(aiScene.Materials, (int)aiScene.NumMaterials, Material.FreeNative, true);

            if (aiScene.RootNode != IntPtr.Zero)
                Node.FreeNative(aiScene.RootNode, true);

            if (aiScene.NumMeshes > 0 && aiScene.Meshes != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiMesh>(aiScene.Meshes, (int)aiScene.NumMeshes, Mesh.FreeNative, true);

            if (aiScene.NumLights > 0 && aiScene.Lights != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiLight>(aiScene.Lights, (int)aiScene.NumLights, Light.FreeNative, true);

            if (aiScene.NumCameras > 0 && aiScene.Cameras != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiCamera>(aiScene.Cameras, (int)aiScene.NumCameras, Camera.FreeNative, true);

            if (aiScene.NumTextures > 0 && aiScene.Textures != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiTexture>(aiScene.Textures, (int)aiScene.NumTextures, EmbeddedTexture.FreeNative, true);

            if (aiScene.NumAnimations > 0 && aiScene.Animations != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiAnimation>(aiScene.Animations, (int)aiScene.NumAnimations, Animation.FreeNative, true);

            if (aiScene.Metadata != IntPtr.Zero)
                Metadata.FreeNative(aiScene.Metadata, true);

            if (freeNative)
                MemoryHelper.FreeMemory(nativeValue);
        }

        #endregion
    }
}
