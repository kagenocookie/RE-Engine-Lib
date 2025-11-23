namespace ReeLib
{
    public enum RszFieldType : uint
    {
        ukn_error = 0,
        ukn_type,
        not_init,
        class_not_found,
        out_of_range,
        Undefined,
        Object,
        Action,
        Struct,
        NativeObject,
        Resource,
        UserData,
        Bool,
        C8,
        C16,
        S8,
        U8,
        UByte = U8,
        S16,
        U16,
        S32,
        U32,
        S64,
        U64,
        F32,
        F64,
        String,
        MBString,
        Enum,
        Uint2,
        Uint3,
        Uint4,
        Int2,
        Int3,
        Int4,
        Float2,
        Float3,
        Float4,
        Float3x3,
        Float3x4,
        Float4x3,
        Float4x4,
        Half2,
        Half4,
        Mat3,
        Mat4,
        Vec2,
        Vec3,
        Vec4,
        VecU4,
        Quaternion,
        Guid,
        Color,
        DateTime,
        AABB,
        Capsule,
        TaperedCapsule,
        Cone,
        Line,
        LineSegment,
        OBB,
        Plane,
        PlaneXZ,
        Point,
        Range,
        RangeI,
        Ray,
        RayY,
        Segment,
        Size,
        Sphere,
        Triangle,
        Cylinder,
        Ellipsoid,
        Area,
        Torus,
        Rect,
        Rect3D,
        Frustum,
        KeyFrame,
        Uri,
        GameObjectRef,
        RuntimeType,
        Sfix,
        Sfix2,
        Sfix3,
        Sfix4,
        Position,
        F16,
        End,
        Data
    };

    enum MAGIC {
        id_SCN = 5129043,
        id_PFB = 4343376,
        id_USR = 5395285,
        id_RCOL = 1280262994,
        id_mfs2 = 846423661,
        id_BHVT = 1414940738,
        id_uvar = 1918989941,
        id_fchr = 1919443814,
    }


    public enum GameName
    {
        unknown,
        re2,
        re2rt,
        re3,
        re3rt,
        re4,
        re7,
        re7rt,
        re8,
        dmc5,
        mhrise,
        sf6,
        dd2,
        gtrick,
        apollo,
        drdr,
        mhwilds,
    }


    public enum GameVersion
    {
        unknown,
        re7 = 1,
        re2 = 2,
        dmc5 = 3,
        re3 = 4,
        re8 = 5,
        mhrise = 6,
        re2rt = 7,
        re3rt = re2rt,
        re7rt = re2rt,
        re4 = 8,
        dd2 = re4,
        mhwilds = re4,
        sf6 = 9,
        gtrick,
        apollo,
        drdr,
    }

    public enum KnownFileFormats
    {
        Unknown,
        /// <summary>.mesh</summary>
        Mesh,
        /// <summary>.tex</summary>
        Texture,
        /// <summary>.scn</summary>
        Scene,
        /// <summary>.pfb</summary>
        Prefab,
        /// <summary>.user</summary>
        UserData,
        /// <summary>.mdf2</summary>
        MaterialDefinition,
        /// <summary>.rcol</summary>
        RequestSetCollider,
        /// <summary>.uvar</summary>
        UserVariables,
        /// <summary>.mot</summary>
        Motion,
        /// <summary>.mottree</summary>
        MotionTree,
        /// <summary>.motlist</summary>
        MotionList,
        /// <summary>.motbank</summary>
        MotionBank,
        /// <summary>.gpumotlist</summary>
        GpuMotionList,
        /// <summary>.cfil</summary>
        CollisionFilter,
        /// <summary>.cdef</summary>
        CollisionDefinition,
        /// <summary>.cmat</summary>
        CollisionMaterial,
        /// <summary>.fol</summary>
        Foliage,
        /// <summary>.mmtr</summary>
        MasterMaterial,
        /// <summary>.gpbf</summary>
        ByteBuffer,
        /// <summary>.gpuc</summary>
        GpuCloth,
        /// <summary>.rtex</summary>
        RenderTexture,
        /// <summary>.efx</summary>
        Effect,
        /// <summary>.tml</summary>
        Timeline,
        /// <summary>.clip</summary>
        Clip,
        /// <summary>.uvs</summary>
        UVSequence,
        /// <summary>.gui</summary>
        GUI,
        /// <summary>.mcol</summary>
        CollisionMesh,
        /// <summary>.fsm</summary>
        Fsm,
        /// <summary>.fsmv2</summary>
        Fsm2,
        /// <summary>.motfsm</summary>
        MotionFsm,
        /// <summary>.motfsm2</summary>
        MotionFsm2,
        /// <summary>.chain</summary>
        Chain,
        /// <summary>.chain2</summary>
        Chain2,
        /// <summary>.lprb</summary>
        LightProbes,
        /// <summary>.prb</summary>
        Probes,
        /// <summary>.aimap, .ainvm, .aivspc, .aiwayp, .aiwaypmgr, .ainvmmgr</summary>
        AIMap,
        /// <summary>.hf</summary>
        HeightField,
        /// <summary>.chf</summary>
        CollisionHeightField,
        /// <summary>.bhvt</summary>
        BehaviorTree,
        /// <summary>.msg</summary>
        Message,
        /// <summary>.coco</summary>
        CompositeCollision,
        /// <summary>.jmap</summary>
        JointMap,
        /// <summary>.rdd</summary>
        Ragdoll,
        /// <summary>.sdftex</summary>
        SDFTexture,
        /// <summary>.rmesh</summary>
        RigidBodyMesh,
        /// <summary>.mpci</summary>
        MaterialPointCloud,
        /// <summary>.ucurve</summary>
        UserCurve,
        /// <summary>.ucurvelist</summary>
        UserCurveList,
        /// <summary>.ccbk</summary>
        CharacterColliderBank,
        /// <summary>.retargetrig</summary>
        RetargetRig,
        /// <summary>.rbs</summary>
        RigidBodySet,
        /// <summary>.jcns</summary>
        JointConstraints,
        /// <summary>.skeleton</summary>
        Skeleton,
        /// <summary>.ikleg2</summary>
        IkLeg2,
        /// <summary>.iklookat2</summary>
        IkLookAt2,
        /// <summary>.ikls</summary>
        IkLegSpine,
        /// <summary>.ikbodyrig</summary>
        IkBodyRig,
        /// <summary>.ikhd</summary>
        IkHand2,
        /// <summary>.ikmulti</summary>
        IkMultiBone,
        /// <summary>.iktrain</summary>
        IkTrain,
        /// <summary>.iktrain2</summary>
        IkTrain2,
        /// <summary>.ikdamage</summary>
        IkDamageAction,
        /// <summary>.iklizard</summary>
        IkLizard,
        /// <summary>.ikwagon</summary>
        IkWagon,
        /// <summary>.fbik</summary>
        FullBodyIKRig,
        /// <summary>.amix</summary>
        AudioMixer,
        /// <summary>.vsrc</summary>
        VibrationSource,
        /// <summary>.fbxskel</summary>
        FbxSkeleton,
        /// <summary>.clsm</summary>
        CollisionSkinningMesh,
        /// <summary>.clsp</summary>
        CollisionShapePreset,
        /// <summary>.wrap</summary>
        WrapDeformer,
        /// <summary>.sfur</summary>
        ShellFur,
        /// <summary>.finf</summary>
        FilterInfo,
        /// <summary>.def</summary>
        DynamicsDefinition,
        /// <summary>.gcp</summary>
        GUIColorPreset,
        /// <summary>.gsty</summary>
        GUIStyleList,
        /// <summary>.gcf</summary>
        GUIConfig,
        /// <summary>.rcf</summary>
        RebeConfig,
        RagdollController,
        /// <summary>.rtmr</summary>
        RayTraceMaterialRedirect,
        /// <summary>.sss</summary>
        SSSProfile,
        /// <summary>.lod</summary>
        Lod,
        /// <summary>.terr</summary>
        Terrain,
        /// <summary>.gml</summary>
        GroundMaterialList,
        /// <summary>.gtl</summary>
        GroundTextureList,
        /// <summary>.grnd</summary>
        Ground,
        /// <summary>.stmesh</summary>
        SpeedTreeMesh,
        /// <summary>.abcmesh</summary>
        AlembicMesh,
        /// <summary>.motcam</summary>
        MotionCamera,
        /// <summary>.mcamlist</summary>
        MotionCameraList,
        /// <summary>.mcambank</summary>
        MotionCameraBank,
        /// <summary>.tmlfsm2</summary>
        TimelineFsm2,
        /// <summary>.oft</summary>
        OutlineFont,
        /// <summary>.fslt</summary>
        FontSlot,
        /// <summary>.ift</summary>
        IconFont,
        /// <summary>.ocioc</summary>
        OpenColorIOConfig,
        /// <summary>.star</summary>
        StarCatalogue,
        /// <summary>.mov</summary>
        Movie,
        /// <summary>.aimapattr</summary>
        AIMapAttribute,
        /// <summary>.swms</summary>
        MemorySettings,
        /// <summary>.sbnk, .bnk</summary>
        SoundBank,
        /// <summary>.spck, .pck</summary>
        SoundPackage,
        /// <summary>.vsdf</summary>
        VfxShader,
        /// <summary>.vsdflist</summary>
        VfxShaderList,
        /// <summary>.efcsv</summary>
        EffectCsv,
        /// <summary>.lfa</summary>
        Lensflare,
        /// <summary>.eem</summary>
        EffectEmitMask,
        /// <summary>.ies</summary>
        IESLight,
        /// <summary>.tmlbld</summary>
        TimelineBlend,
        /// <summary>.jointlodgroup</summary>
        JointLODGroup,
        /// <summary>.jntexprgraph</summary>
        JointExpressionGraph,
        /// <summary>.vmap</summary>
        MeshVertexMapping,
        /// <summary>.ziva</summary>
        ZivaRT,
        /// <summary>.zivacomb</summary>
        ZivaCombiner,
        /// <summary>.dlg</summary>
        Dialogue,
        /// <summary>.dlgcf</summary>
        DialogueConfig,
        /// <summary>.dlglist</summary>
        DialogueList,
        /// <summary>.dlgtml</summary>
        DialogueTimeline,
        DialogueTimelineList,
        /// <summary>.clrp</summary>
        ClothResetPose,
        /// <summary>.cset</summary>
        ColliderSet,
        /// <summary>.fpolygon</summary>
        FreePolygon,
        /// <summary>.sts</summary>
        Strands,
        /// <summary>.htex</summary>
        HeightTexture,
        /// <summary>.ncf</summary>
        NetworkConfig,
        /// <summary>.psop</summary>
        PSOPatch,
        /// <summary>.sst</summary>
        SetStateList,
        /// <summary>.chainwnd</summary>
        ChainWind,
        /// <summary>.pog</summary>
        PointGraph,
        /// <summary>.poglst</summary>
        PointGraphList,
        /// <summary>.csdf</summary>
        CfxShader,
        /// <summary>.sdf</summary>
        Shader,
        /// <summary>.fxct</summary>
        EffectCollision,
        /// <summary>.ord</summary>
        OodleNetworkRuntimeData,
        /// <summary>.rcfg</summary>
        RenderConfigAsset,

        /// <remarks>Deprecated in favor of <see cref="EffectCollision"/></remarks>
        fxct,
        /// <remarks>Deprecated in favor of <see cref="OodleNetworkRuntimeData"/></remarks>
        ord,
        /// <remarks>Deprecated in favor of <see cref="PointGraph"/></remarks>
        pog,
        /// <remarks>Deprecated in favor of <see cref="PointGraphList"/></remarks>
        poglst,
        /// <remarks>Deprecated in favor of <see cref="RenderConfigAsset"/></remarks>
        rcfg,
        /// <remarks>Deprecated in favor of <see cref="Shader"/></remarks>
        sdf,

        TimelineBase, // Timeline or Clip
        MotionBase, // Motion or MotionTree or MotionList or GpuMotionList or MotionCamera or MotionCameraList
        DynamicsBase, // RigidBodyMesh or HeightField
        BehaviorTreeBase, // BehaviorTree or Fsm2
    }
}
