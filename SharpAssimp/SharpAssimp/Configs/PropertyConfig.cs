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
using System.Numerics;
using System.Text;

namespace SharpAssimp.Configs
{
    /// <summary>
    /// Base property config.
    /// </summary>
    public abstract class PropertyConfig
    {
        private string m_name;

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string Name => m_name;

        /// <summary>
        /// Creates a new property config that has no active Assimp property store.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        protected PropertyConfig(string name)
        {
            m_name = name;
        }

        /// <summary>
        /// Sets the current value to the default value.
        /// </summary>
        public abstract void SetDefaultValue();

        /// <summary>
        /// Applies the property value to the given Assimp property store.
        /// </summary>
        /// <param name="propStore">Assimp property store</param>
        internal void ApplyValue(IntPtr propStore)
        {
            OnApplyValue(propStore);
        }

        /// <summary>
        /// Applies the property value to the given Assimp property store.
        /// </summary>
        /// <param name="propStore">Assimp property store</param>
        protected abstract void OnApplyValue(IntPtr propStore);
    }

    /// <summary>
    /// Describes an integer configuration property.
    /// </summary>
    public class IntegerPropertyConfig : PropertyConfig
    {
        private int m_value;
        private int m_defaultValue;

        /// <summary>
        /// Gets the property value.
        /// </summary>
        public int Value
        {
            get => m_value;
            set => m_value = value;
        }

        /// <summary>
        /// Gets the default property value.
        /// </summary>
        public int DefaultValue => m_defaultValue;

        /// <summary>
        /// Constructs a new IntengerPropertyConfig.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Property value</param>
        public IntegerPropertyConfig(string name, int value)
            : this(name, value, 0) { }

        /// <summary>
        /// constructs a new IntegerPropertyConfig with a default value.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Property value</param>
        /// <param name="defaultValue">The default property value</param>
        public IntegerPropertyConfig(string name, int value, int defaultValue)
            : base(name)
        {
            m_value = value;
            m_defaultValue = defaultValue;
        }

        /// <summary>
        /// Sets the current value to the default value.
        /// </summary>
        public override void SetDefaultValue()
        {
            m_value = m_defaultValue;
        }

        /// <summary>
        /// Applies the property value to the given Assimp property store.
        /// </summary>
        /// <param name="propStore">Assimp property store</param>
        protected override void OnApplyValue(IntPtr propStore)
        {
            if (propStore != IntPtr.Zero)
            {
                AssimpLibrary.Instance.SetImportPropertyInteger(propStore, Name, m_value);
            }
        }
    }

    /// <summary>
    /// Describes a float configuration property.
    /// </summary>
    public class FloatPropertyConfig : PropertyConfig
    {
        private float m_value;
        private float m_defaultValue;

        /// <summary>
        /// Gets the property value.
        /// </summary>
        public float Value
        {
            get => m_value;
            set => m_value = value;
        }

        /// <summary>
        /// Gets the default property value.
        /// </summary>
        public float DefaultValue => m_defaultValue;

        /// <summary>
        /// Constructs a new FloatPropertyConfig.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Property value</param>
        public FloatPropertyConfig(string name, float value)
            : this(name, value, 0.0f) { }

        /// <summary>
        /// Constructs a new FloatPropertyConfig with a default value.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Property value</param>
        /// <param name="defaultValue">The default property value</param>
        public FloatPropertyConfig(string name, float value, float defaultValue)
            : base(name)
        {
            m_value = value;
            m_defaultValue = defaultValue;
        }

        /// <summary>
        /// Sets the current value to the default value.
        /// </summary>
        public override void SetDefaultValue()
        {
            m_value = m_defaultValue;
        }

        /// <summary>
        /// Applies the property value to the given Assimp property store.
        /// </summary>
        /// <param name="propStore">Assimp property store</param>
        protected override void OnApplyValue(IntPtr propStore)
        {
            if (propStore != IntPtr.Zero)
            {
                AssimpLibrary.Instance.SetImportPropertyFloat(propStore, Name, m_value);
            }
        }
    }

    /// <summary>
    /// Describes a <see cref="Matrix4x4"/> configuration property.
    /// </summary>
    public class MatrixPropertyConfig : PropertyConfig
    {
        private Matrix4x4 m_value;
        private Matrix4x4 m_defaultValue;

        /// <summary>
        /// Gets the property value.
        /// </summary>
        public Matrix4x4 Value
        {
            get => m_value;
            set => m_value = value;
        }

        /// <summary>
        /// Gets the default property value.
        /// </summary>
        public Matrix4x4 DefaultValue => m_defaultValue;

        /// <summary>
        /// Constructs a new MatrixPropertyConfig.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Property value</param>
        public MatrixPropertyConfig(string name, Matrix4x4 value)
            : this(name, value, Matrix4x4.Identity) { }

        /// <summary>
        /// Constructs a new MatrixPropertyConfig.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Property value</param>
        /// <param name="defaultValue">The default property value</param>
        public MatrixPropertyConfig(string name, Matrix4x4 value, Matrix4x4 defaultValue)
            : base(name)
        {
            m_value = value;
            m_defaultValue = defaultValue;
        }

        /// <summary>
        /// Sets the current value to the default value.
        /// </summary>
        public override void SetDefaultValue()
        {
            m_value = m_defaultValue;
        }

        /// <summary>
        /// Applies the property value to the given Assimp property store.
        /// </summary>
        /// <param name="propStore">Assimp property store</param>
        protected override void OnApplyValue(IntPtr propStore)
        {
            if (propStore != IntPtr.Zero)
            {
                AssimpLibrary.Instance.SetImportPropertyMatrix(propStore, Name, m_value);
            }
        }
    }

    /// <summary>
    /// Describes a boolean configuration property.
    /// </summary>
    public class BooleanPropertyConfig : PropertyConfig
    {
        private bool m_value;
        private bool m_defaultValue;

        /// <summary>
        /// Gets the property value.
        /// </summary>
        public bool Value
        {
            get => m_value;
            set => m_value = value;
        }

        /// <summary>
        /// Gets the default property value.
        /// </summary>
        public bool DefaultValue => m_defaultValue;

        /// <summary>
        /// Constructs a new BooleanPropertyConfig.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Property value</param>
        public BooleanPropertyConfig(string name, bool value)
            : this(name, value, false) { }

        /// <summary>
        /// Constructs a new BooleanPropertyConfig with a default value.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Property value</param>
        /// <param name="defaultValue">The default property value</param>
        public BooleanPropertyConfig(string name, bool value, bool defaultValue)
            : base(name)
        {
            m_value = value;
            m_defaultValue = defaultValue;
        }

        /// <summary>
        /// Sets the current value to the default value.
        /// </summary>
        public override void SetDefaultValue()
        {
            m_value = m_defaultValue;
        }

        /// <summary>
        /// Applies the property value to the given Assimp property store.
        /// </summary>
        /// <param name="propStore">Assimp property store</param>
        protected override void OnApplyValue(IntPtr propStore)
        {
            if (propStore != IntPtr.Zero)
            {
                int aiBool = (m_value) ? 1 : 0;
                AssimpLibrary.Instance.SetImportPropertyInteger(propStore, Name, aiBool);
            }
        }
    }

    /// <summary>
    /// Describes a string configuration property.
    /// </summary>
    public class StringPropertyConfig : PropertyConfig
    {
        private string m_value;
        private string m_defaultValue;

        /// <summary>
        /// Gets the property value.
        /// </summary>
        public string Value
        {
            get => m_value;
            set => m_value = value;
        }

        /// <summary>
        /// Gets the default property value.
        /// </summary>
        public string DefaultValue => m_defaultValue;

        /// <summary>
        /// Constructs a new StringPropertyConfig.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Property value</param>
        public StringPropertyConfig(string name, string value)
            : this(name, value, string.Empty) { }

        /// <summary>
        /// Constructs a new StringPropertyConfig with a default value.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Property value</param>
        /// <param name="defaultValue">The default property value</param>
        public StringPropertyConfig(string name, string value, string defaultValue)
            : base(name)
        {
            m_value = value;
            m_defaultValue = defaultValue;
        }

        /// <summary>
        /// Sets the current value to the default value.
        /// </summary>
        public override void SetDefaultValue()
        {
            m_value = m_defaultValue;
        }

        /// <summary>
        /// Applies the property value to the given Assimp property store.
        /// </summary>
        /// <param name="propStore">Assimp property store</param>
        protected override void OnApplyValue(IntPtr propStore)
        {
            if (propStore != IntPtr.Zero)
            {
                AssimpLibrary.Instance.SetImportPropertyString(propStore, Name, m_value);
            }
        }

        /// <summary>
        /// Convience method for constructing a whitespace delimited name list.
        /// </summary>
        /// <param name="names">Array of names</param>
        /// <returns>White-space delimited list as a string</returns>
        protected static string ProcessNames(string[] names)
        {
            if (names == null || names.Length == 0)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();
            foreach (string name in names)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    builder.Append(name);
                    builder.Append(' ');
                }
            }
            return builder.ToString();
        }
    }

    #region Library settings

    /// <summary>
    /// Configuration to enable time measurements. If enabled, each
    /// part of the loading process is timed and logged. Default value is false.
    /// </summary>
    public sealed class MeasureTimeConfig : BooleanPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by MeasureTimeConfig.
        /// </summary>
        public static string MeasureTimeConfigName => AiConfigs.AI_CONFIG_GLOB_MEASURE_TIME;

        /// <summary>
        /// Constructs a new MeasureTimeConfig.
        /// </summary>
        /// <param name="measureTime">True if the loading process should be timed or not.</param>
        public MeasureTimeConfig(bool measureTime)
            : base(MeasureTimeConfigName, measureTime, false) { }
    }

    /// <summary>
    /// Configuration to set Assimp's multithreading policy. Possible
    /// values are -1 to let Assimp decide, 0 to disable multithreading, or
    /// any number larger than zero to force a specific number of threads. This
    /// is only a hint and may be ignored by Assimp. Default value is -1.
    /// </summary>
    public sealed class MultithreadingConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by MultithreadingConfig.
        /// </summary>
        public static string MultithreadingConfigName => AiConfigs.AI_CONFIG_GLOB_MULTITHREADING;

        /// <summary>
        /// Constructs a new MultithreadingConfig.
        /// </summary>
        /// <param name="value">A value of -1 will let Assimp decide,
        /// a value of zero to disable multithreading, and a value greater than zero
        /// to force a specific number of threads.</param>
        public MultithreadingConfig(int value)
            : base(MultithreadingConfigName, value, -1) { }
    }

    /// <summary>
    /// Global setting to disable generation of skeleton dummy meshes. These are generated as a visualization aid
    /// in cases which the input data contains no geometry, but only animation data. So the geometry are visualizing
    /// the bones. Default value is false.
    /// </summary>
    public sealed class NoSkeletonMeshesConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by NoSkeletonMeshConfig.
        /// </summary>
        public static string NoSkeletonMeshesConfigName => AiConfigs.AI_CONFIG_IMPORT_NO_SKELETON_MESHES;

        /// <summary>
        /// Constructs a new NoSkeletonMeshConfig.
        /// </summary>
        /// <param name="disableDummySkeletonMeshes">True if dummy skeleton mesh generation should be disabled, false otherwise.</param>
        public NoSkeletonMeshesConfig(bool disableDummySkeletonMeshes)
            : base(NoSkeletonMeshesConfigName, disableDummySkeletonMeshes, false) { }
    }

    #endregion

    #region Post Processing Settings

    /// <summary>
    /// Configuration to set the maximum angle that may be between two vertex tangents/bitangents
    /// when they are smoothed during the step to calculate the tangent basis. The default
    /// value is 45 degrees.
    /// </summary>
    public sealed class TangentSmoothingAngleConfig : FloatPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by TangentSmoothingAngleConfig.
        /// </summary>
        public static string TangentSmoothingAngleConfigName => AiConfigs.AI_CONFIG_PP_CT_MAX_SMOOTHING_ANGLE;

        /// <summary>
        /// Constructs a new TangentSmoothingAngleConfig.
        /// </summary>
        /// <param name="angle">Smoothing angle, in degrees.</param>
        public TangentSmoothingAngleConfig(float angle)
            : base(TangentSmoothingAngleConfigName, Math.Min(angle, 175.0f), 45.0f) { }
    }

    /// <summary>
    /// Configuration to set the maximum angle between two face normals at a vertex when
    /// they are smoothed during the step to calculate smooth normals. This is frequently
    /// called the "crease angle". The maximum and default value is 175 degrees.
    /// </summary>
    public sealed class NormalSmoothingAngleConfig : FloatPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by NormalSmoothingAngleConfig.
        /// </summary>
        public static string NormalSmoothingAngleConfigName => AiConfigs.AI_CONFIG_PP_GSN_MAX_SMOOTHING_ANGLE;

        /// <summary>
        /// Constructs a new NormalSmoothingAngleConfig.
        /// </summary>
        /// <param name="angle">Smoothing angle, in degrees.</param>
        public NormalSmoothingAngleConfig(float angle)
            : base(NormalSmoothingAngleConfigName, Math.Min(angle, 175.0f), 175.0f) { }
    }

    /// <summary>
    /// Configuration to set the colormap (palette) to be used to decode embedded textures in MDL (Quake or 3DG5)
    /// files. This must be a valid path to a file. The file is 768 (256 * 3) bytes alrge and contains
    /// RGB triplets for each of the 256 palette entries. If the file is not found, a
    /// default palette (from Quake 1) is used. The default value is "colormap.lmp".
    /// </summary>
    public sealed class MDLColorMapConfig : StringPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by MDLColorMapConfig.
        /// </summary>
        public static string MDLColorMapConfigName => AiConfigs.AI_CONFIG_IMPORT_MDL_COLORMAP;

        /// <summary>
        /// Constructs a new MDLColorMapConfig.
        /// </summary>
        /// <param name="fileName">Colormap filename</param>
        public MDLColorMapConfig(string fileName)
            : base(MDLColorMapConfigName, (string.IsNullOrEmpty(fileName)) ? "colormap.lmp" : fileName, "colormap.lmp") { }
    }

    /// <summary>
    /// Configuration for the the <see cref="PostProcessSteps.RemoveRedundantMaterials"/> step
    /// to determine what materials to keep. If a material matches one of these names it will not
    /// be modified or removed by the post processing step. Default is an empty string.
    /// </summary>
    public sealed class MaterialExcludeListConfig : StringPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by MaterialExcludeListConfig.
        /// </summary>
        public static string MaterialExcludeListConfigName => AiConfigs.AI_CONFIG_PP_RRM_EXCLUDE_LIST;

        /// <summary>
        /// Constructs a new MaterialExcludeListConfig. Material names containing whitespace
        /// <c>must</c> be enclosed in single quotation marks.
        /// </summary>
        /// <param name="materialNames">List of material names that will not be modified or replaced by the remove redundant materials post process step.</param>
        public MaterialExcludeListConfig(string[] materialNames)
            : base(MaterialExcludeListConfigName, ProcessNames(materialNames), string.Empty) { }
    }

    /// <summary>
    /// Configuration for the <see cref="PostProcessSteps.PreTransformVertices"/> step
    /// to keep the scene hierarchy. Meshes are moved to worldspace, but no optimization is performed
    /// where meshes with the same materials are not joined. This option can be useful
    /// if you have a scene hierarchy that contains important additional information
    /// which you intend to parse. The default value is false.
    /// </summary>
    public sealed class KeepSceneHierarchyConfig : BooleanPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by KeepSceneHierarchyConfig.
        /// </summary>
        public static string KeepSceneHierarchyConfigName => AiConfigs.AI_CONFIG_PP_PTV_KEEP_HIERARCHY;

        /// <summary>
        /// Constructs a new KeepHierarchyConfig. 
        /// </summary>
        /// <param name="keepHierarchy">True to keep the hierarchy, false otherwise.</param>
        public KeepSceneHierarchyConfig(bool keepHierarchy)
            : base(KeepSceneHierarchyConfigName, keepHierarchy, false) { }
    }

    /// <summary>
    /// Configuration for the <see cref="PostProcessSteps.PreTransformVertices"/> step
    /// to normalize all vertex components into the -1...1 range. The default value is
    /// false.
    /// </summary>
    public sealed class NormalizeVertexComponentsConfig : BooleanPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by NormalizeVertexComponentsConfig.
        /// </summary>
        public static string NormalizeVertexComponentsConfigName => AiConfigs.AI_CONFIG_PP_PTV_NORMALIZE;

        /// <summary>
        /// Constructs a new NormalizeVertexComponentsConfig.
        /// </summary>
        /// <param name="normalizeVertexComponents">True if the post process step should normalize vertex components, false otherwise.</param>
        public NormalizeVertexComponentsConfig(bool normalizeVertexComponents)
            : base(NormalizeVertexComponentsConfigName, normalizeVertexComponents, false) { }
    }

    /// <summary>
    /// Configuration for the <see cref="PostProcessSteps.FindDegenerates"/> step to
    /// remove degenerted primitives from the import immediately. The default value is false,
    /// where degenerated triangles are converted to lines, and degenerated lines to points.
    /// </summary>
    public sealed class RemoveDegeneratePrimitivesConfig : BooleanPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by RemoveDegeneratePrimitivesConfig.
        /// </summary>
        public static string RemoveDegeneratePrimitivesConfigName => AiConfigs.AI_CONFIG_PP_FD_REMOVE;

        /// <summary>
        /// Constructs a new RemoveDegeneratePrimitivesConfig.
        /// </summary>
        /// <param name="removeDegenerates">True if the post process step should remove degenerate primitives, false otherwise.</param>
        public RemoveDegeneratePrimitivesConfig(bool removeDegenerates)
            : base(RemoveDegeneratePrimitivesConfigName, removeDegenerates, false) { }
    }

    /// <summary>
    /// Configuration for the <see cref="PostProcessSteps.FindDegenerates"/> step. If true, the area of the triangles are checked
    /// to see if they are greater than 1e-6. If so, the triangle is removed if <see cref="RemoveDegeneratePrimitivesConfig"/> is set to true.
    /// </summary>
    public sealed class RemoveDegeneratePrimitivesCheckAreaConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by RemoveDegeneratePrimitivesCheckAreaConfig.
        /// </summary>
        public static string RemoveDegeneratePrimitivesCheckAreaConfigName => AiConfigs.AI_CONFIG_PP_FD_CHECKAREA;

        /// <summary>
        /// Constructs a new RemoveDegeneratePrimitivesCheckAreaConfig.
        /// </summary>
        /// <param name="checkArea">True if the post process step should check the area of triangles when finding degenerate primitives, false otherwise.</param>
        public RemoveDegeneratePrimitivesCheckAreaConfig(bool checkArea)
            : base(RemoveDegeneratePrimitivesCheckAreaConfigName, checkArea, false) { }
    }

    /// <summary>
    /// Configuration for the <see cref="PostProcessSteps.OptimizeGraph"/> step
    /// to preserve nodes matching a name in the given list. Nodes that match the names in the list
    /// will not be modified or removed. Identifiers containing whitespaces
    /// <c>must</c> be enclosed in single quotation marks. The default value is an
    /// empty string.
    /// </summary>
    public sealed class NodeExcludeListConfig : StringPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by NodeExcludeListConfig.
        /// </summary>
        public static string NodeExcludeListConfigName => AiConfigs.AI_CONFIG_PP_OG_EXCLUDE_LIST;

        /// <summary>
        /// Constructs a new NodeExcludeListConfig.
        /// </summary>
        /// <param name="nodeNames">List of node names</param>
        public NodeExcludeListConfig(params string[] nodeNames)
            : base(NodeExcludeListConfigName, ProcessNames(nodeNames), string.Empty) { }
    }

    /// <summary>
    /// Configuration for the <see cref="PostProcessSteps.SplitLargeMeshes"/> step 
    /// that specifies the maximum number of triangles a mesh can contain. The
    /// default value is MeshTriangleLimitConfigDefaultValue.
    /// </summary>
    public sealed class MeshTriangleLimitConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by MeshTriangleLimitConfig.
        /// </summary>
        public static string MeshTriangleLimitConfigName => AiConfigs.AI_CONFIG_PP_SLM_TRIANGLE_LIMIT;

        /// <summary>
        /// Gets the defined default limit value, this corresponds to the
        /// <see cref="AiDefines.AI_SLM_DEFAULT_MAX_TRIANGLES"/> constant.
        /// </summary>
        public static int MeshTriangleLimitConfigDefaultValue => AiDefines.AI_SLM_DEFAULT_MAX_TRIANGLES;

        /// <summary>
        /// Constructs a new MeshTriangleLimitConfig.
        /// </summary>
        /// <param name="maxTriangleLimit">Max number of triangles a mesh can contain.</param>
        public MeshTriangleLimitConfig(int maxTriangleLimit)
            : base(MeshTriangleLimitConfigName, maxTriangleLimit, MeshTriangleLimitConfigDefaultValue) { }
    }

    /// <summary>
    /// Configuration for the <see cref="PostProcessSteps.SplitLargeMeshes"/> step
    /// that specifies the maximum number of vertices a mesh can contain. The
    /// default value is MeshVertexLimitConfigDefaultValue.
    /// </summary>
    public sealed class MeshVertexLimitConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by MeshVertexLimitConfig.
        /// </summary>
        public static string MeshVertexLimitConfigName => AiConfigs.AI_CONFIG_PP_SLM_VERTEX_LIMIT;

        /// <summary>
        /// Gets the defined default limit value, this corresponds to the
        /// <see cref="AiDefines.AI_SLM_DEFAULT_MAX_VERTICES"/> constant.
        /// </summary>
        public static int MeshVertexLimitConfigDefaultValue => AiDefines.AI_SLM_DEFAULT_MAX_VERTICES;

        /// <summary>
        /// Constructs a new MeshVertexLimitConfig.
        /// </summary>
        /// <param name="maxVertexLimit">Max number of vertices a mesh can contain.</param>
        public MeshVertexLimitConfig(int maxVertexLimit)
            : base(MeshVertexLimitConfigName, maxVertexLimit, MeshVertexLimitConfigDefaultValue) { }
    }

    /// <summary>
    /// Configuration for the <see cref="PostProcessSteps.LimitBoneWeights"/> step
    /// that specifies the maximum number of bone weights per vertex. The default
    /// value is VertexBoneWeightLimitConfigDefaultValue.
    /// </summary>
    public sealed class VertexBoneWeightLimitConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// gets the string name used by VertexBoneWeightLimitConfig.
        /// </summary>
        public static string VertexBoneWeightLimitConfigName => AiConfigs.AI_CONFIG_PP_LBW_MAX_WEIGHTS;

        /// <summary>
        /// Gets the defined default limit value, this corresponds to the
        /// <see cref="AiDefines.AI_LBW_MAX_WEIGHTS"/> constant.
        /// </summary>
        public static int VertexBoneWeightLimitConfigDefaultValue => AiDefines.AI_LBW_MAX_WEIGHTS;

        /// <summary>
        /// Constructs a new VertexBoneWeightLimitConfig.
        /// </summary>
        /// <param name="maxBoneWeights">Max number of bone weights per vertex.</param>
        public VertexBoneWeightLimitConfig(int maxBoneWeights)
            : base(VertexBoneWeightLimitConfigName, maxBoneWeights, VertexBoneWeightLimitConfigDefaultValue) { }
    }

    /// <summary>
    /// Configuration for the <see cref="PostProcessSteps.ImproveCacheLocality"/> step
    /// that specifies the size of the post-transform vertex cache. The size is
    /// given in number of vertices and the default value is VertexCacheSizeConfigDefaultValue.
    /// </summary>
    public sealed class VertexCacheSizeConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by VertexCacheConfig.
        /// </summary>
        public static string VertexCacheSizeConfigName => AiConfigs.AI_CONFIG_PP_ICL_PTCACHE_SIZE;

        /// <summary>
        /// Gets the defined default vertex cache size, this corresponds to 
        /// the <see cref="AiDefines.PP_ICL_PTCACHE_SIZE"/>.
        /// </summary>
        public static int VertexCacheSizeConfigDefaultValue => AiDefines.PP_ICL_PTCACHE_SIZE;

        /// <summary>
        /// Constructs a new VertexCacheSizeConfig.
        /// </summary>
        /// <param name="vertexCacheSize">Size of the post-transform vertex cache, in number of vertices.</param>
        public VertexCacheSizeConfig(int vertexCacheSize)
            : base(VertexCacheSizeConfigName, vertexCacheSize, VertexCacheSizeConfigDefaultValue) { }
    }

    /// <summary>
    /// Configuration for the <see cref="PostProcessSteps.RemoveComponent"/> step that
    /// specifies which parts of the data structure is to be removed. If no valid mesh
    /// remains after the step, the import fails. The default value i <see cref="ExcludeComponent.None"/>.
    /// </summary>
    public sealed class RemoveComponentConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by RemoveComponentConfig.
        /// </summary>
        public static string RemoveComponentConfigName => AiConfigs.AI_CONFIG_PP_RVC_FLAGS;

        /// <summary>
        /// Constructs a new RemoveComponentConfig.
        /// </summary>
        /// <param name="componentsToExclude">Bit-wise combination of components to exclude.</param>
        public RemoveComponentConfig(ExcludeComponent componentsToExclude)
            : base(RemoveComponentConfigName, (int)componentsToExclude, (int)ExcludeComponent.None) { }
    }

    /// <summary>
    /// Configuration for the <see cref="PostProcessSteps.SortByPrimitiveType"/> step that
    /// specifies which primitive types are to be removed by the step. Specifying all
    /// primitive types is illegal. The default value is zero specifying none.
    /// </summary>
    public sealed class SortByPrimitiveTypeConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by SortByPrimitiveTypeConfig.
        /// </summary>
        public static string SortByPrimitiveTypeConfigName => AiConfigs.AI_CONFIG_PP_SBP_REMOVE;

        /// <summary>
        /// Constructs a new SortByPrimitiveTypeConfig.
        /// </summary>
        /// <param name="typesToRemove">Bit-wise combination of primitive types to remove</param>
        public SortByPrimitiveTypeConfig(PrimitiveType typesToRemove)
            : base(SortByPrimitiveTypeConfigName, (int)typesToRemove, 0) { }
    }

    /// <summary>
    /// Configuration for the <see cref="PostProcessSteps.FindInvalidData"/> step that
    /// specifies the floating point accuracy for animation values, specifically
    /// the episilon during comparisons. The default value is 0.0f.
    /// </summary>
    public sealed class AnimationAccuracyConfig : FloatPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by AnimationAccuracyConfig.
        /// </summary>
        public static string AnimationAccuracyConfigName => AiConfigs.AI_CONFIG_PP_FID_ANIM_ACCURACY;

        /// <summary>
        /// Constructs a new AnimationAccuracyConfig.
        /// </summary>
        /// <param name="episilon">Episilon for animation value comparisons.</param>
        public AnimationAccuracyConfig(float episilon)
            : base(AnimationAccuracyConfigName, episilon, 0.0f) { }
    }

    /// <summary>
    /// Configuration for the <see cref="PostProcessSteps.FindInvalidData"/> step. Set to true to
    /// ignore texture coordinates. This may be useful if you have to assign different kinds of textures,
    /// like seasonally variable ones - one for summer and one for winter. Default is false.
    /// </summary>
    public sealed class IgnoreTextureCoordinatesConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by IgnoreTextureCoordinatesConfig.
        /// </summary>
        public static string IgnoreTextureCoordinatesConfigName => AiConfigs.AI_CONFIG_PP_FID_IGNORE_TEXTURECOORDS;

        /// <summary>
        /// Constructs a new IgnoreTextureCoordinatesConfig.
        /// </summary>
        /// <param name="ignoreTexCoords">True if texture coordinates should be ignored, false otherwise.</param>
        public IgnoreTextureCoordinatesConfig(bool ignoreTexCoords)
            : base(IgnoreTextureCoordinatesConfigName, ignoreTexCoords, false) { }
    }

    /// <summary>
    /// Configuration for the <see cref="PostProcessSteps.TransformUVCoords"/> step that
    /// specifies which UV transformations are to be evaluated. The default value
    /// is for all combinations (scaling, rotation, translation).
    /// </summary>
    public sealed class TransformUVConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by TransformUVConfig.
        /// </summary>
        public static string TransformUVConfigName => AiConfigs.AI_CONFIG_PP_TUV_EVALUATE;

        /// <summary>
        /// Constructs a new TransformUVConfig.
        /// </summary>
        /// <param name="transformFlags">Bit-wise combination specifying which UV transforms that should be evaluated.</param>
        public TransformUVConfig(UVTransformFlags transformFlags)
            : base(TransformUVConfigName, (int)transformFlags, (int)AiDefines.AI_UVTRAFO_ALL) { }
    }

    /// <summary>
    /// Configuration that is a hint to Assimp to favor speed against import quality. Enabling this
    /// option may result in faster loading, or it may not. It is just a hint to loaders
    /// and post-process steps to use faster code paths if possible. The default value is false.
    /// </summary>
    public sealed class FavorSpeedConfig : BooleanPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by FavorSpeedConfig.
        /// </summary>
        public static string FavorSpeedConfigName => AiConfigs.AI_CONFIG_FAVOUR_SPEED;

        /// <summary>
        /// Constructs a new FavorSpeedConfig.
        /// </summary>
        /// <param name="favorSpeed">True if Assimp should favor speed at the expense of quality, false otherwise.</param>
        public FavorSpeedConfig(bool favorSpeed)
            : base(FavorSpeedConfigName, favorSpeed, false) { }
    }

    /// <summary>
    /// Configures the maximum bone count per mesh for the <see cref="PostProcessSteps.SplitByBoneCount"/> step. Meshes are
    /// split until the maximum number of bones is reached.
    /// </summary>
    public sealed class MaxBoneCountConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by MaxBoneCountConfig.
        /// </summary>
        public static string MaxBoneCountConfigName => AiConfigs.AI_CONFIG_PP_SBBC_MAX_BONES;

        /// <summary>
        /// Constructs a new MaxBoneCountConfig.
        /// </summary>
        /// <param name="maxBones">The maximum bone count.</param>
        public MaxBoneCountConfig(int maxBones)
            : base(MaxBoneCountConfigName, maxBones, AiDefines.AI_SBBC_DEFAULT_MAX_BONES) { }
    }

    /// <summary>
    /// Configures which texture channel is used for tangent space computations. The channel must exist or an error will be raised.
    /// </summary>
    public sealed class TangentTextureChannelIndexConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by TangentTextureChannelIndexConfig.
        /// </summary>
        public static string TangentTextureChannelIndexConfigName => AiConfigs.AI_CONFIG_PP_CT_TEXTURE_CHANNEL_INDEX;

        /// <summary>
        /// Constructs a new TangentTextureChannelIndexConfig.
        /// </summary>
        /// <param name="textureChannelIndex">The zero-based texture channel index.</param>
        public TangentTextureChannelIndexConfig(int textureChannelIndex)
            : base(TangentTextureChannelIndexConfigName, textureChannelIndex, 0) { }
    }

    /// <summary>
    /// Configures the <see cref="PostProcessSteps.Debone"/> threshold that is used to determine what bones are removed.
    /// </summary>
    public sealed class DeboneThresholdConfig : FloatPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by DeboneThresholdConfig.
        /// </summary>
        public static string DeboneThresholdConfigName => AiConfigs.AI_CONFIG_PP_DB_THRESHOLD;

        /// <summary>
        /// Constructs a new DeboneThresholdConfig.
        /// </summary>
        /// <param name="threshold">The debone threshold.</param>
        public DeboneThresholdConfig(float threshold)
            : base(DeboneThresholdConfigName, threshold, 1.0f) { }
    }


    /// <summary>
    /// Configuration that requires all bones to qualify for deboning before any are removed.
    /// </summary>
    public sealed class DeboneAllOrNoneConfig : BooleanPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by DeboneAllOrNoneConfig.
        /// </summary>
        public static string DeboneAllOrNoneConfigName => AiConfigs.AI_CONFIG_PP_DB_ALL_OR_NONE;

        /// <summary>
        /// Constructs a new DeboneAllOrNoneConfig.
        /// </summary>
        /// <param name="allOrNone">True if all are required, false if none need to qualify.</param>
        public DeboneAllOrNoneConfig(bool allOrNone)
            : base(DeboneAllOrNoneConfigName, allOrNone, false) { }
    }

    /// <summary>
    /// Configuration for <see cref="PostProcessSteps.PreTransformVertices"/> that sets a user defined matrix as the scene root node transformation before
    /// transforming vertices. Default value is the identity matrix.
    /// </summary>
    public sealed class RootTransformationConfig : MatrixPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by RootTransformationConfig.
        /// </summary>
        public static string RootTransformationConfigName => AiConfigs.AI_CONFIG_PP_PTV_ROOT_TRANSFORMATION;

        /// <summary>
        /// Constructs a new RootTransformationConfig.
        /// </summary>
        /// <param name="rootTransform">Root transformation matrix to be set to the root scene node during the pretransform post process step.</param>
        public RootTransformationConfig(Matrix4x4 rootTransform)
            : base(RootTransformationConfigName, rootTransform, Matrix4x4.Identity) { }

        /// <summary>
        /// Applies the property value to the given Assimp property store.
        /// </summary>
        /// <param name="propStore">Assimp property store</param>
        protected override void OnApplyValue(IntPtr propStore)
        {
            if (propStore != IntPtr.Zero)
            {
                //Technically this is TWO configs, a boolean that we want to do it and a config with the actual matrix. Most likely if we're setting the actual matrix, then we really do want
                //to apply the root transformation, so this config actually represents two configs.
                AssimpLibrary.Instance.SetImportPropertyInteger(propStore, AiConfigs.AI_CONFIG_PP_PTV_ADD_ROOT_TRANSFORMATION, 1); //TRUE = 1
                AssimpLibrary.Instance.SetImportPropertyMatrix(propStore, RootTransformationConfigName, Value);
            }
        }
    }

    /// <summary>
    /// Configures the <see cref="PostProcessSteps.GlobalScale"/> step to scale the entire scene by a certain amount. Some importers provide a mechanism to define a scaling unit for the model,
    /// which this processing step can utilize. Default is 1.0.
    /// </summary>
    /// <seealso cref="SharpAssimp.Configs.FloatPropertyConfig" />
    public sealed class GlobalScaleConfig : FloatPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by GlobalScaleConfig.
        /// </summary>
        public static string GlobalScaleConfigName => AiConfigs.AI_CONFIG_GLOBAL_SCALE_FACTOR_KEY;

        /// <summary>
        /// Constructs a new GlobalScaleConfig.
        /// </summary>
        /// <param name="globalScale">Value to scale the entire scene by.</param>
        public GlobalScaleConfig(float globalScale)
            : base(GlobalScaleConfigName, globalScale, 1.0f) { }
    }

    /// <summary>
    /// Applies an application-specific scaling to the <see cref="GlobalScaleConfig"/> to allow for backwards compatibility. Default is 1.0.
    /// </summary>
    public sealed class AppScaleConfig : FloatPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by AppScaleConfig.
        /// </summary>
        public static string AppScaleConfigName => AiConfigs.AI_CONFIG_APP_SCALE_KEY;

        /// <summary>
        /// Constructs a new AppScaleConfig.
        /// </summary>
        /// <param name="appScale">Value to scale the global scale by.</param>
        public AppScaleConfig(float appScale)
            : base(AppScaleConfigName, appScale, 1.0f) { }
    }

    #endregion

    #region Importer Settings

    /// <summary>
    /// Sets the vertex animation keyframe to be imported. Assimp does not support vertex keyframes (only
    /// bone animation is supported). the library reads only one keyframe with vertex animations. By default this is the
    /// first frame. This config sets the "global" keyframe that will be imported. There are other configs
    /// for specific importers that will override the global setting.
    /// </summary>
    public sealed class GlobalKeyFrameImportConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by GlobalKeyFrameImportConfig.
        /// </summary>
        public static string GlobalKeyFrameImportConfigName => AiConfigs.AI_CONFIG_IMPORT_GLOBAL_KEYFRAME;

        /// <summary>
        /// Constructs a new GlobalKeyFrameImportConfig.
        /// </summary>
        /// <param name="keyFrame">Keyframe index</param>
        public GlobalKeyFrameImportConfig(int keyFrame)
            : base(GlobalKeyFrameImportConfigName, keyFrame, 0) { }
    }

    /// <summary>
    /// Sets the vertex animation keyframe to be imported. Assimp does not support vertex keyframes (only
    /// bone animation is supported). the library reads only one keyframe with vertex animations. By default this is the
    /// first frame. This config sets the global override for the MD3 format.
    /// </summary>
    public sealed class MD3KeyFrameImportConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by MD3KeyFrameImportConfig.
        /// </summary>
        public static string MD3KeyFrameImportConfigName => AiConfigs.AI_CONFIG_IMPORT_MD3_KEYFRAME;

        /// <summary>
        /// Constructs a new MD3KeyFrameImportConfig.
        /// </summary>
        /// <param name="keyFrame">Keyframe index</param>
        public MD3KeyFrameImportConfig(int keyFrame)
            : base(MD3KeyFrameImportConfigName, keyFrame, 0) { }
    }

    /// <summary>
    /// Sets the vertex animation keyframe to be imported. Assimp does not support vertex keyframes (only
    /// bone animation is supported). the library reads only one keyframe with vertex animations. By default this is the
    /// first frame. This config sets the global override for the MD2 format.
    /// </summary>
    public sealed class MD2KeyFrameImportConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by MD2KeyFrameImportConfig.
        /// </summary>
        public static string MD2KeyFrameImportConfigName => AiConfigs.AI_CONFIG_IMPORT_MD2_KEYFRAME;

        /// <summary>
        /// Constructs a new MD2KeyFrameImportConfig.
        /// </summary>
        /// <param name="keyFrame">Keyframe index</param>
        public MD2KeyFrameImportConfig(int keyFrame)
            : base(MD2KeyFrameImportConfigName, keyFrame, 0) { }
    }

    /// <summary>
    /// Sets the vertex animation keyframe to be imported. Assimp does not support vertex keyframes (only
    /// bone animation is supported). the library reads only one keyframe with vertex animations. By default this is the
    /// first frame. This config sets the global override for the MDL format.
    /// </summary>
    public sealed class MDLKeyFrameImportConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by MDLKeyFrameImportConfig.
        /// </summary>
        public static string MDLKeyFrameImportConfigName => AiConfigs.AI_CONFIG_IMPORT_MDL_KEYFRAME;

        /// <summary>
        /// Constructs a new MDLKeyFrameImportConfig.
        /// </summary>
        /// <param name="keyFrame">Keyframe index</param>
        public MDLKeyFrameImportConfig(int keyFrame)
            : base(MDLKeyFrameImportConfigName, keyFrame, 0) { }
    }

    /// <summary>
    /// Sets the vertex animation keyframe to be imported. Assimp does not support vertex keyframes (only
    /// bone animation is supported). the library reads only one keyframe with vertex animations. By default this is the
    /// first frame. This config sets the global override for the SMD format.
    /// </summary>
    public sealed class SMDKeyFrameImportConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by SMDKeyFrameImportConfig.
        /// </summary>
        public static string SMDKeyFrameImportConfigName => AiConfigs.AI_CONFIG_IMPORT_SMD_KEYFRAME;

        /// <summary>
        /// Constructs a new SMDKeyFrameImportConfig.
        /// </summary>
        /// <param name="keyFrame">Keyframe index</param>
        public SMDKeyFrameImportConfig(int keyFrame)
            : base(SMDKeyFrameImportConfigName, keyFrame, 0) { }
    }

    /// <summary>
    /// Sets the vertex animation keyframe to be imported. Assimp does not support vertex keyframes (only
    /// bone animation is supported). the library reads only one keyframe with vertex animations. By default this is the
    /// first frame. This config sets the global override for the Unreal format.
    /// </summary>
    public sealed class UnrealKeyFrameImportConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by UnrealKeyFrameImportConfig.
        /// </summary>
        public static string UnrealKeyFrameImportConfigName => AiConfigs.AI_CONFIG_IMPORT_UNREAL_KEYFRAME;

        /// <summary>
        /// Constructs a new UnrealKeyFrameImportConfig.
        /// </summary>
        /// <param name="keyFrame">Keyframe index</param>
        public UnrealKeyFrameImportConfig(int keyFrame)
            : base(UnrealKeyFrameImportConfigName, keyFrame, 0) { }
    }

    /// <summary>
    /// Configures the AC loader to collect all surfaces which have the "Backface cull" flag set in separate
    /// meshes. The default value is true.
    /// </summary>
    public sealed class ACSeparateBackfaceCullConfig : BooleanPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by ACSeparateBackfaceCullConfig.
        /// </summary>
        public static string ACSeparateBackfaceCullConfigName => AiConfigs.AI_CONFIG_IMPORT_AC_SEPARATE_BFCULL;

        /// <summary>
        /// Constructs a new ACSeparateBackfaceCullConfig.
        /// </summary>
        /// <param name="separateBackfaces">True if all surfaces that have the "backface cull" flag set should be collected in separate meshes, false otherwise.</param>
        public ACSeparateBackfaceCullConfig(bool separateBackfaces)
            : base(ACSeparateBackfaceCullConfigName, separateBackfaces, true) { }
    }

    /// <summary>
    /// Configures whether the AC loader evaluates subdivision surfaces (indicated by the presence
    /// of the 'subdiv' attribute in the file). By default, Assimp performs
    /// the subdivision using the standard Catmull-Clark algorithm. The default value is true.
    /// </summary>
    public sealed class ACEvaluateSubdivisionConfig : BooleanPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by ACEvaluateSubdivisionConfig.
        /// </summary>
        public static string ACEvaluateSubdivisionConfigName => AiConfigs.AI_CONFIG_IMPORT_AC_EVAL_SUBDIVISION;

        /// <summary>
        /// Constructs a new ACEvaluateSubdivisionConfig.
        /// </summary>
        /// <param name="evaluateSubdivision">True if the AC loader should evaluate subdivisions, false otherwise.</param>
        public ACEvaluateSubdivisionConfig(bool evaluateSubdivision)
            : base(ACEvaluateSubdivisionConfigName, evaluateSubdivision, true) { }
    }

    /// <summary>
    /// Configures the UNREAL 3D loader to separate faces with different surface flags (e.g. two-sided vs single-sided).
    /// The default value is true.
    /// </summary>
    public sealed class UnrealHandleFlagsConfig : BooleanPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by UnrealHandleFlagsConfig.
        /// </summary>
        public static string UnrealHandleFlagsConfigName => AiConfigs.AI_CONFIG_IMPORT_UNREAL_HANDLE_FLAGS;

        /// <summary>
        /// Constructs a new UnrealHandleFlagsConfig.
        /// </summary>
        /// <param name="handleFlags">True if the unreal loader should separate faces with different surface flags, false otherwise.</param>
        public UnrealHandleFlagsConfig(bool handleFlags)
            : base(UnrealHandleFlagsConfigName, handleFlags, true) { }
    }

    /// <summary>
    /// Configures the terragen import plugin to compute UV's for terrains, if
    /// they are not given. Furthermore, a default texture is assigned. The default value is false.
    /// <para>UV coordinates for terrains are so simple to compute that you'll usually 
    /// want to compute them on your own, if you need them. This option is intended for model viewers which
    /// want to offer an easy way to apply textures to terrains.</para>
    /// </summary>
    public sealed class TerragenComputeTexCoordsConfig : BooleanPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by TerragenComputeTexCoordsConfig.
        /// </summary>
        public static string TerragenComputeTexCoordsConfigName => AiConfigs.AI_CONFIG_IMPORT_TER_MAKE_UVS;

        /// <summary>
        /// Constructs a new TerragenComputeTexCoordsConfig.
        /// </summary>
        /// <param name="computeTexCoords">True if terran UV coordinates should be computed, false otherwise.</param>
        public TerragenComputeTexCoordsConfig(bool computeTexCoords)
            : base(TerragenComputeTexCoordsConfigName, computeTexCoords, false) { }
    }

    /// <summary>
    /// Configures the ASE loader to always reconstruct normal vectors basing on the smoothing groups
    /// loaded from the file. Some ASE files carry invalid normals, others don't. The default value is true.
    /// </summary>
    public sealed class ASEReconstructNormalsConfig : BooleanPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by ASEReconstructNormalsConfig.
        /// </summary>
        public static string ASEReconstructNormalsConfigName => AiConfigs.AI_CONFIG_IMPORT_ASE_RECONSTRUCT_NORMALS;

        /// <summary>
        /// Constructs a new ASEReconstructNormalsConfig.
        /// </summary>
        /// <param name="reconstructNormals">True if normals should be re-computed, false otherwise.</param>
        public ASEReconstructNormalsConfig(bool reconstructNormals)
            : base(ASEReconstructNormalsConfigName, reconstructNormals, true) { }
    }

    /// <summary>
    /// Configures the M3D loader to detect and process multi-part Quake player models. These models
    /// usually consit of three files, lower.md3, upper.md3 and head.md3. If this propery is
    /// set to true, Assimp will try to load and combine all three files if one of them is loaded. The
    /// default value is true.
    /// </summary>
    public sealed class MD3HandleMultiPartConfig : BooleanPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by MD3HandleMultiPartConfig.
        /// </summary>
        public static string MD3HandleMultiPartConfigName => AiConfigs.AI_CONFIG_IMPORT_MD3_HANDLE_MULTIPART;

        /// <summary>
        /// Constructs a new MD3HandleMultiPartConfig.
        /// </summary>
        /// <param name="handleMultiParts">True if the split files should be loaded and combined, false otherwise.</param>
        public MD3HandleMultiPartConfig(bool handleMultiParts)
            : base(MD3HandleMultiPartConfigName, handleMultiParts, true) { }
    }

    /// <summary>
    /// Tells the MD3 loader which skin files to load. When loading MD3 files, Assimp checks
    /// whether a file named "md3_file_name"_"skin_name".skin exists. These files are used by
    /// Quake III to be able to assign different skins (e.g. red and blue team) to models. 'default', 'red', 'blue'
    /// are typical skin names. The default string value is "default".
    /// </summary>
    public sealed class MD3SkinNameConfig : StringPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by MD3SkinNameConfig.
        /// </summary>
        public static string MD3SkinNameConfigName => AiConfigs.AI_CONFIG_IMPORT_MD3_SKIN_NAME;

        /// <summary>
        /// Constructs a new MD3SkinNameConfig.
        /// </summary>
        /// <param name="skinName">The skin name.</param>
        public MD3SkinNameConfig(string skinName)
            : base(MD3SkinNameConfigName, skinName, "default") { }
    }

    /// <summary>
    /// Specifies the Quake 3 shader file to be used for a particular MD3 file. This can be a full path or
    /// relative to where all MD3 shaders reside. the default string value is an empty string.
    /// </summary>
    public sealed class MD3ShaderSourceConfig : StringPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by MD3ShaderSourceConfig.
        /// </summary>
        public static string MD3ShaderSourceConfigName => AiConfigs.AI_CONFIG_IMPORT_MD3_SHADER_SRC;

        /// <summary>
        /// Constructs a new MD3ShaderSourceConfig.
        /// </summary>
        /// <param name="shaderFile">The shader file.</param>
        public MD3ShaderSourceConfig(string shaderFile)
            : base(MD3ShaderSourceConfigName, shaderFile, string.Empty) { }
    }

    /// <summary>
    /// Configures the LWO loader to load just one layer from the model.
    /// <para>LWO files consist of layers and in some cases it could be useful to load only one of them.
    /// This property can be either a string - which specifies the name of the layer - or an integer - the index
    /// of the layer. If the property is not set then the whole LWO model is loaded. Loading fails
    /// if the requested layer is not vailable. The layer index is zero-based and the layer name may not be empty</para>
    /// The default value is false (all layers are loaded).
    /// </summary>
    public sealed class LWOImportOneLayerConfig : BooleanPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by LWOImportOneLayerConfig.
        /// </summary>
        public static string LWOImportOneLayerConfigName => AiConfigs.AI_CONFIG_IMPORT_LWO_ONE_LAYER_ONLY;

        /// <summary>
        /// Constructs a new LWOImportOneLayerConfig.
        /// </summary>
        /// <param name="importOneLayerOnly">True if only one layer should be imported, false if all layers should be imported.</param>
        public LWOImportOneLayerConfig(bool importOneLayerOnly)
            : base(LWOImportOneLayerConfigName, importOneLayerOnly, false) { }
    }

    /// <summary>
    /// Configures the MD5 loader to not load the MD5ANIM file for a MD5MESH file automatically.
    /// The default value is false.
    /// <para>The default strategy is to look for a file with the same name but with the MD5ANIm extension
    /// in the same directory. If it is found it is loaded and combined with the MD5MESH file. This configuration
    /// option can be used to disable this behavior.</para>
    /// </summary>
    public sealed class MD5NoAnimationAutoLoadConfig : BooleanPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by MD5NoAnimationAutoLoadConfig.
        /// </summary>
        public static string MD5NoAnimationAutoLoadConfigName => AiConfigs.AI_CONFIG_IMPORT_MD5_NO_ANIM_AUTOLOAD;

        /// <summary>
        /// Constructs a new MD5NoAnimationAutoLoadConfig.
        /// </summary>
        /// <param name="noAutoLoadAnim">True if animations should not be automatically loaded, false if they should be.</param>
        public MD5NoAnimationAutoLoadConfig(bool noAutoLoadAnim)
            : base(MD5NoAnimationAutoLoadConfigName, noAutoLoadAnim, false) { }
    }

    /// <summary>
    /// Defines the beginning of the time range for which the LWS loader evaluates animations and computes
    /// AiNodeAnim's. The default value is the one taken from the file.
    /// <para>Assimp provides full conversion of Lightwave's envelope system, including pre and post
    /// conditions. The loader computes linearly subsampled animation channels with the frame rate
    /// given in the LWS file. This property defines the start time.</para>
    /// <para>Animation channels are only generated if a node has at least one envelope with more than one key
    /// assigned. This property is given in frames where '0' is the first. By default,
    /// if this property is not set, the importer takes the animation start from the input LWS
    /// file ('FirstFrame' line)</para>
    /// </summary>
    public sealed class LWSAnimationStartConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by LWSAnimationStartConfig.
        /// </summary>
        public static string LWSAnimationStartConfigName => AiConfigs.AI_CONFIG_IMPORT_LWS_ANIM_START;

        /// <summary>
        /// Constructs a new LWSAnimationStartConfig.
        /// </summary>
        /// <param name="animStart">Beginning of the time range</param>
        public LWSAnimationStartConfig(int animStart)
            : base(LWSAnimationStartConfigName, animStart, -1) { } //TODO: Verify the default value to tell the loader to use the value from the file
    }

    /// <summary>
    /// Defines the ending of the time range for which the LWS loader evaluates animations and computes
    /// AiNodeAnim's. The default value is the one taken from the file
    /// <para>Assimp provides full conversion of Lightwave's envelope system, including pre and post
    /// conditions. The loader computes linearly subsampled animation channels with the frame rate
    /// given in the LWS file. This property defines the end time.</para>
    /// <para>Animation channels are only generated if a node has at least one envelope with more than one key
    /// assigned. This property is given in frames where '0' is the first. By default,
    /// if this property is not set, the importer takes the animation end from the input LWS
    /// file.</para>
    /// </summary>
    public sealed class LWSAnimationEndConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by LWSAnimationEndConfig.
        /// </summary>
        public static string LWSAnimationEndConfigName => AiConfigs.AI_CONFIG_IMPORT_LWS_ANIM_END;

        /// <summary>
        /// Constructs a new LWSAnimationEndConfig.
        /// </summary>
        /// <param name="animEnd">Ending of the time range</param>
        public LWSAnimationEndConfig(int animEnd)
            : base(LWSAnimationEndConfigName, animEnd, -1) { } //TODO: Verify the default value to tell the loader to use the value from the file.
    }

    /// <summary>
    /// Defines the output frame rate of the IRR loader.
    /// <para>IRR animations are difficult to convert for Assimp and there will always be
    /// a loss of quality. This setting defines how many keys per second are returned by the converter.</para>
    /// The default value is 100 frames per second.
    /// </summary>
    public sealed class IRRAnimationFrameRateConfig : IntegerPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by IRRAnimationFrameRateConfig.
        /// </summary>
        public static string IRRAnimationFrameRateConfigName => AiConfigs.AI_CONFIG_IMPORT_IRR_ANIM_FPS;

        /// <summary>
        /// Constructs a new IRRAnimationFramerateConfig.
        /// </summary>
        /// <param name="frameRate">Number of frames per second to output.</param>
        public IRRAnimationFrameRateConfig(int frameRate)
            : base(IRRAnimationFrameRateConfigName, frameRate, 100) { }
    }

    /// <summary>
    /// The Ogre importer will try to load this MaterialFile. Ogre meshes reference with material names, this does not tell Assimp
    /// where the file is located. Assimp will try to find the source file in the following order: [material-name].material, [mesh-filename-base].material,
    /// and lastly the material name defined by this config property. The default value is "Scene.Material".
    /// </summary>
    public sealed class OgreMaterialFileConfig : StringPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by OgreMaterialFileConfig.
        /// </summary>
        public static string OgreMaterialFileConfigName => AiConfigs.AI_CONFIG_IMPORT_OGRE_MATERIAL_FILE;

        /// <summary>
        /// Constructs a new OgreMaterialFileConfig.
        /// </summary>
        /// <param name="materialFileName">Material file name to load.</param>
        public OgreMaterialFileConfig(string materialFileName)
            : base(OgreMaterialFileConfigName, materialFileName, "Scene.Material") { }
    }

    /// <summary>
    /// The Ogre importer will detect the texture usage from the filename. Normally a texture is loaded as a color map, if no target is specified
    /// in the material file. If this is enabled, then Assimp will try to detect the type from the texture filename postfix: 
    /// <list type="bullet">
    /// <item><description>Normal Maps: _n, _nrm, _nrml, _normal, _normals, _normalmap</description></item>
    /// <item><description>Specular Maps: _s, _spec, _specular, _specularmap</description></item>
    /// <item><description>Light Maps: _l, _light, _lightmap, _occ, _occlusion</description></item>
    /// <item><description>Displacement Maps: _dis, _displacement</description></item>
    /// </list>
    /// The matching is case insensitive. Postfix is taken between the last "_" and last ".". The default behavior is to detect type from lower cased
    /// texture unit name by matching against: normalmap, specularmap, lightmap, and displacementmap. For both cases if no match is found then,
    /// <see cref="TextureType.Diffuse"/> is used. The default value is false.
    /// </summary>
    public sealed class OgreTextureTypeFromFilenameConfig : BooleanPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by OgreTextureTypeFromFilenameConfig.
        /// </summary>
        public static string OgreTextureTypeFromFilenameConfigName => AiConfigs.AI_CONFIG_IMPORT_OGRE_TEXTURETYPE_FROM_FILENAME;

        /// <summary>
        /// Constructs a new OgreTextureTypeFromFilenameConfig.
        /// </summary>
        /// <param name="fileNameDefinesTextureUsage">True if the filename defines texture usage, false otherwise.</param>
        public OgreTextureTypeFromFilenameConfig(bool fileNameDefinesTextureUsage)
            : base(OgreTextureTypeFromFilenameConfigName, fileNameDefinesTextureUsage, false) { }
    }

    /// <summary>
    /// Specifies whether the IFC loader skips over IfcSpace elements. IfcSpace elements (and their geometric representations) are used to represent free space in a building story.
    /// </summary>
    public sealed class IFCSkipSpaceRepresentationsConfig : BooleanPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by IFCSkipSpaceRepresentationsConfig.
        /// </summary>
        public static string IFCSkipSpaceRepresentationsConfigName => AiConfigs.AI_CONFIG_IMPORT_IFC_SKIP_SPACE_REPRESENTATIONS;

        /// <summary>
        /// Constructs a new IFCSkipSpaceRepresentationsConfig.
        /// </summary>
        /// <param name="skipSpaceRepresentations">True if the IfcSpace elements are skipped, false if otherwise.</param>
        public IFCSkipSpaceRepresentationsConfig(bool skipSpaceRepresentations)
            : base(IFCSkipSpaceRepresentationsConfigName, skipSpaceRepresentations, true) { }
    }

    /// <summary>
    /// Specifies whether the IFC loader will use its own, custom triangulation algorithm to triangulate wall and floor meshes. If this is set to false,
    /// walls will be either triangulated by the post process triangulation or will be passed through as huge polygons with faked holes (e.g. holes that are connected
    /// with the outer boundary using a dummy edge). It is highly recommended to leave this property set to true as the default post process has some known
    /// issues with these kind of polygons.
    /// </summary>
    public sealed class IFCUseCustomTriangulationConfig : BooleanPropertyConfig
    {

        /// <summary>
        /// Gets the string name used by IFCUseCustomTriangulationConfig.
        /// </summary>
        public static string IFCUseCustomTriangulationConfigName => AiConfigs.AI_CONFIG_IMPORT_IFC_CUSTOM_TRIANGULATION;

        /// <summary>
        /// Constructs a new IFCUseCustomTriangulationConfig.
        /// </summary>
        /// <param name="useCustomTriangulation">True if the loader should use its own triangulation routine for walls/floors, false otherwise.</param>
        public IFCUseCustomTriangulationConfig(bool useCustomTriangulation)
            : base(IFCUseCustomTriangulationConfigName, useCustomTriangulation, true) { }
    }

    /// <summary>
    /// Specifies the tessellation conic angle for IFC smoothing curves. Accepted range of values is between [5, 120]
    /// </summary>
    public sealed class IFCSmoothingAngleConfig : FloatPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by IFCSmoothingAngleConfig.
        /// </summary>
        public static string IFCSmoothingAngleConfigName => AiConfigs.AI_CONFIG_IMPORT_IFC_SMOOTHING_ANGLE;

        /// <summary>
        /// Constructs a new IFCSmoothingAngleConfig.
        /// </summary>
        /// <param name="angle">Smoothing angle when tessellating curves. Needs to be in the range of [5, 120].</param>
        public IFCSmoothingAngleConfig(float angle)
            : base(IFCSmoothingAngleConfigName, angle, 10.0f) { }
    }

    /// <summary>
    /// Specifies the tessellation for IFC cylindrical shapes. E.g. the number of segments used to approximate a circle. Accepted range of values is between [3, 180].
    /// </summary>
    public sealed class IFCCylindricalTessellationConfig : IntegerPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by IFCCylindricalTessellationConfig.
        /// </summary>
        public static string IFCCylindricalTessellationConfigName => AiConfigs.AI_CONFIG_IMPORT_IFC_CYLINDRICAL_TESSELLATION;

        /// <summary>
        /// Constructs a new IFCCylindricalTessellationConfig.
        /// </summary>
        /// <param name="tessellation">Tessellation of cylindrical shapes (e.g. the number of segments used to approximate a circle). Needs to be in the range of [3, 180].</param>
        public IFCCylindricalTessellationConfig(int tessellation)
            : base(IFCCylindricalTessellationConfigName, tessellation, 32) { }
    }

    /// <summary>
    /// Specifies whether the collada loader will ignore the up direction. Default is false.
    /// </summary>
    public sealed class ColladaIgnoreUpDirectionConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by ColladaIgnoreUpDirectionConfig.
        /// </summary>
        public static string ColladaIgnoreUpDirectionConfigName => AiConfigs.AI_CONFIG_IMPORT_COLLADA_IGNORE_UP_DIRECTION;

        /// <summary>
        /// Constructs a new ColladaIgnoreUpDirectionConfig.
        /// </summary>
        /// <param name="ignoreUpDirection">True if the loader should ignore the up direction, false otherwise.</param>
        public ColladaIgnoreUpDirectionConfig(bool ignoreUpDirection)
            : base(ColladaIgnoreUpDirectionConfigName, ignoreUpDirection, false) { }
    }

    /// <summary>
    /// Specifies whether the Collada loader should use Collada names as node names.
    /// If this property is set to true, the Collada names will be used as the node name. The behavior is to use the id tag (resp. sid tag, if no id tag is present) instead.
    /// Default is false.
    /// </summary>
    public sealed class ColladaUseColladaNamesConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by ColladaUseColladaNamesConfig.
        /// </summary>
        public static string ColladaUseColladaNamesConfigName => AiConfigs.AI_CONFIG_IMPORT_COLLADA_USE_COLLADA_NAMES;

        /// <summary>
        /// Constructs a new ColladaUseColladaNamesConfig.
        /// </summary>
        /// <param name="useColladaNames">True if collada names should be used as node names, false otherwise.</param>
        public ColladaUseColladaNamesConfig(bool useColladaNames)
            : base(ColladaUseColladaNamesConfigName, useColladaNames, false) { }
    }

    /// <summary>
    /// Specifies whether the FBX importer will merge all geometry layers present in the source file or import only the first. Default is true.
    /// </summary>
    public sealed class FBXImportAllGeometryLayersConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by FBXImportAllGeometryLayersConfig.
        /// </summary>
        public static string FBXImportAllGeometryLayersConfigName => AiConfigs.AI_CONFIG_IMPORT_FBX_READ_ALL_GEOMETRY_LAYERS;

        /// <summary>
        /// Constructs a new FBXImportAllGeometryLayersConfig.
        /// </summary>
        /// <param name="importAllGeometryLayers">True if all geometry layers should be merged, false otherwise to take only the first layer.</param>
        public FBXImportAllGeometryLayersConfig(bool importAllGeometryLayers)
            : base(FBXImportAllGeometryLayersConfigName, importAllGeometryLayers, true) { }
    }

    /// <summary>
    /// Specifies whether the FBX importer will import all materials present in the source file or take only the referenced materials,
    /// if the importer is configured to import materials at all. Otherwise this will have no effect. Default is false.
    /// </summary>
    public sealed class FBXImportAllMaterialsConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by FBXImportAllMaterialsConfig.
        /// </summary>
        public static string FBXImportAllMaterialsConfigName => AiConfigs.AI_CONFIG_IMPORT_FBX_READ_ALL_MATERIALS;

        /// <summary>
        /// Constructs a new FBXImportAllMaterialsConfig.
        /// </summary>
        /// <param name="importAllMaterials">True if the FBX importer should import ALL materials even if not referenced, false otherwise (take only the referenced materials).</param>
        public FBXImportAllMaterialsConfig(bool importAllMaterials)
            : base(FBXImportAllMaterialsConfigName, importAllMaterials, false) { }
    }

    /// <summary>
    /// Specifies whether the FBX importer will import materials. Default is true.
    /// </summary>
    public sealed class FBXImportMaterialsConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by FBXImportMaterialsConfig.
        /// </summary>
        public static string FBXImportMaterialsConfigName => AiConfigs.AI_CONFIG_IMPORT_FBX_READ_MATERIALS;

        /// <summary>
        /// Constructs a new FBXImportMaterialsConfig.
        /// </summary>
        /// <param name="importMaterials">True if the FBX importer should import materials, false otherwise.</param>
        public FBXImportMaterialsConfig(bool importMaterials)
            : base(FBXImportMaterialsConfigName, importMaterials, true) { }
    }

    /// <summary>
    /// Specifies whether the FBX importer will import embedded textures. Default is true.
    /// </summary>
    /// <seealso cref="SharpAssimp.Configs.BooleanPropertyConfig" />
    public sealed class FBXImportEmbeddedTexturesConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by FBXImportEmbeddedTexturesConfig.
        /// </summary>
        public static string FBXImportEmbeddedTexturesConfigName => AiConfigs.AI_CONFIG_IMPORT_FBX_READ_TEXTURES;

        /// <summary>
        /// Constructs a new FBXImportEmbeddedTexturesConfig.
        /// </summary>
        /// <param name="importTextures">True if the FBX importer should import embedded textures, false otherwise.</param>
        public FBXImportEmbeddedTexturesConfig(bool importTextures)
            : base(FBXImportEmbeddedTexturesConfigName, importTextures, true) { }
    }

    /// <summary>
    /// Specifies if the FBX importer should search for embedded loaded textures, where no embedded texture data is provided. Default is false.
    /// </summary>
    public sealed class FBXImportEmbeddedTexturesLegacyNamingConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by FBXImportSearchEmbeddedTexturesConfig.
        /// </summary>
        public static string FBXImportEmbeddedTexturesLegacyNamingConfigName => AiConfigs.AI_CONFIG_IMPORT_FBX_EMBEDDED_TEXTURES_LEGACY_NAMING;

        /// <summary>
        /// Constructs a new FBXImportSearchEmbeddedTexturesConfig.
        /// </summary>
        /// <param name="searchEmbeddedTextures">True if the FBX importer should search for embedded loaded textures, where no embedded texture data is provided.</param>
        public FBXImportEmbeddedTexturesLegacyNamingConfig(bool searchEmbeddedTextures)
            : base(FBXImportEmbeddedTexturesLegacyNamingConfigName, searchEmbeddedTextures, false) { }
    }

    /// <summary>
    /// Specifies whether the FBX importer will import cameras. Default is true.
    /// </summary>
    public sealed class FBXImportCamerasConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by FBXImportCamerasConfig.
        /// </summary>
        public static string FBXImportCamerasConfigName => AiConfigs.AI_CONFIG_IMPORT_FBX_READ_CAMERAS;

        /// <summary>
        /// Constructs a new FBXImportCamerasConfig.
        /// </summary>
        /// <param name="importCameras">True if the FBX importer should import cameras, false otherwise.</param>
        public FBXImportCamerasConfig(bool importCameras)
            : base(FBXImportCamerasConfigName, importCameras, true) { }
    }

    /// <summary>
    /// Specifies whether the FBX importer will import lights. Default is true.
    /// </summary>
    public sealed class FBXImportLightsConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by FBXImportLightsConfig.
        /// </summary>
        public static string FBXImportLightsConfigName => AiConfigs.AI_CONFIG_IMPORT_FBX_READ_LIGHTS;

        /// <summary>
        /// Constructs a new FBXImportLightsConfig.
        /// </summary>
        /// <param name="importLights">True if the FBX importer should import lights, false otherwise.</param>
        public FBXImportLightsConfig(bool importLights)
            : base(FBXImportLightsConfigName, importLights, true) { }
    }

    /// <summary>
    /// Specifies whether the FBX importer will import animations. Default is true.
    /// </summary>
    public sealed class FBXImportAnimationsConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by FBXImportAnimationsConfig.
        /// </summary>
        public static string FBXImportAnimationsConfigName => AiConfigs.AI_CONFIG_IMPORT_FBX_READ_ANIMATIONS;

        /// <summary>
        /// Constructs a new FBXImportAnimationsConfig.
        /// </summary>
        /// <param name="importAnimations">True if the FBX importer should import animations, false otherwise.</param>
        public FBXImportAnimationsConfig(bool importAnimations)
            : base(FBXImportAnimationsConfigName, importAnimations, true) { }
    }

    /// <summary>
    /// Specifies whether the FBX importer will act in strict mode in which only the FBX 2013
    /// format is supported and any other sub formats are rejected. FBX 2013 is the primary target for the importer, so this
    /// format is best supported and well-tested. Default is false.
    /// </summary>
    public sealed class FBXStrictModeConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by FBXStrictModeConfig.
        /// </summary>
        public static string FBXStrictModeConfigName => AiConfigs.AI_CONFIG_IMPORT_FBX_STRICT_MODE;

        /// <summary>
        /// Constructs a new FBXStrictModeConfig.
        /// </summary>
        /// <param name="useStrictMode">True if FBX strict mode should be used, false otherwise.</param>
        public FBXStrictModeConfig(bool useStrictMode)
            : base(FBXStrictModeConfigName, useStrictMode, false) { }
    }

    /// <summary>
    /// Specifies whether the FBX importer will preserve pivot points for transformations (as extra nodes). If set to false, pivots
    /// and offsets will be evaluated whenever possible. Default value is true.
    /// </summary>
    public sealed class FBXPreservePivotsConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by FBXPreservePivotsConfig.
        /// </summary>
        public static string FBXPreservePivotsConfigName => AiConfigs.AI_CONFIG_IMPORT_FBX_PRESERVE_PIVOTS;

        /// <summary>
        /// Constructs a new FBXPreservePivotsConfig.
        /// </summary>
        /// <param name="preservePivots">True if pivots should be preserved, false otherwise.</param>
        public FBXPreservePivotsConfig(bool preservePivots)
            : base(FBXPreservePivotsConfigName, preservePivots, true) { }
    }

    /// <summary>
    /// Specifies whether the importer will drop empty animation curves or animation curves which match the bind pose 
    /// transformation over their entire defined range. Default value is true.
    /// </summary>
    public sealed class FBXOptimizeEmptyAnimationCurvesConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by FBXOptimizeEmptyAnimationCurvesConfig.
        /// </summary>
        public static string FBXOptimizeEmptyAnimationCurvesConfigName => AiConfigs.AI_CONFIG_IMPORT_FBX_OPTIMIZE_EMPTY_ANIMATION_CURVES;

        /// <summary>
        /// Constructs a new FBXOptimizeEmptyAnimationCurvesConfig.
        /// </summary>
        /// <param name="optimizeEmptyAnimations">True if empty animation curves should be dropped, false otherwise.</param>
        public FBXOptimizeEmptyAnimationCurvesConfig(bool optimizeEmptyAnimations)
            : base(FBXOptimizeEmptyAnimationCurvesConfigName, optimizeEmptyAnimations, true) { }
    }

    /// <summary>
    /// Specifies whether the importer shall convert the unit from centimeter (cm) to meter (m). Default value is false.
    /// </summary>
    public sealed class FBXConvertToMetersConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by FBXConvertToMetersConfig.
        /// </summary>
        public static string FBXConvertToMetersConfigName => AiConfigs.AI_CONFIG_FBX_CONVERT_TO_M;

        /// <summary>
        /// Constructs a new FBXConvertToMetersConfig.
        /// </summary>
        /// <param name="convertToMeters">True if the importer converts the unit from cm to m, false if do not do a conversion.</param>
        public FBXConvertToMetersConfig(bool convertToMeters)
            : base(FBXConvertToMetersConfigName, convertToMeters, false) { }
    }

    /// <summary>
    /// Specifies whether the importer will load multiple animations. Default value is true.
    /// </summary>
    public sealed class SmdLoadAnimationListConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by SmdLoadAnimationListConfig.
        /// </summary>
        public static string SmdLoadAnimationListConfigName => AiConfigs.AI_CONFIG_IMPORT_SMD_LOAD_ANIMATION_LIST;

        /// <summary>
        /// Constructs a new SmdLoadAnimationListConfig.
        /// </summary>
        /// <param name="loadAnimList">True if the importer should load multiple animations, false if only one animation should be loaded.</param>
        public SmdLoadAnimationListConfig(bool loadAnimList)
            : base(SmdLoadAnimationListConfigName, loadAnimList, true) { }
    }

    /// <summary>
    /// Specifies whether the importer removes empty bones or not. Empty bones are often used to define connections for other models (e.g.
    /// attachment points). Default value is true.
    /// </summary>
    public sealed class RemoveEmptyBonesConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by RemoveEmptyBonesConfig.
        /// </summary>
        public static string RemoveEmptyBonesConfigName => AiConfigs.AI_CONFIG_IMPORT_REMOVE_EMPTY_BONES;

        /// <summary>
        /// Constructs a new RemoveEmptyBonesConfig.
        /// </summary>
        /// <param name="removeEmptyBones">True if the importer should remove empty bones, false if they should be kept.</param>
        public RemoveEmptyBonesConfig(bool removeEmptyBones)
            : base(RemoveEmptyBonesConfigName, removeEmptyBones, true) { }
    }

    #endregion

    #region Exporter Settings

    /// <summary>
    /// Specifies if the X-file exporter should use 64-bit doubles rather than 32-bit floats.
    /// </summary>
    public sealed class XFileUseDoublesConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by XFileUseDoublesConfig.
        /// </summary>
        public static string XFileUseDoublesConfigName => AiConfigs.AI_CONFIG_EXPORT_XFILE_64BIT;

        /// <summary>
        /// Constructs a new XFileUseDoublesConfig.
        /// </summary>
        /// <param name="useDoubles">True if the x file uses 64-bit double values rather than 32-bit float values.</param>
        public XFileUseDoublesConfig(bool useDoubles)
            : base(XFileUseDoublesConfigName, useDoubles, false) { }
    }

    /// <summary>
    /// Specifies if the export process should disable a validation step that would remove data that does not contain faces. This will
    /// enable point cloud data to be exported, since the 3D data is a collection of vertices without face data.
    /// </summary>
    public sealed class ExportPointCloudsConfig : BooleanPropertyConfig
    {
        /// <summary>
        /// Gets the string name used by ExportPointCloudsConfig.
        /// </summary>
        public static string ExportPointCloudsConfigName => AiConfigs.AI_CONFIG_EXPORT_POINT_CLOUDS;

        /// <summary>
        /// Constructs a new ExportPointCloudConfig.
        /// </summary>
        /// <param name="exportPointCloud">True if the exporter should treat vertices not grouped in faces as point clouds, false otherwise.</param>
        public ExportPointCloudsConfig(bool exportPointCloud)
            : base(ExportPointCloudsConfigName, exportPointCloud, false) { }
    }

    #endregion
}
