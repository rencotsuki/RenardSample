/*
 * Mediapipe - Landmarkポイントの定義
 */

namespace SignageHADO.Tracking
{
    public static class MediapipeLandmark
    {
        #region Face

        public const int FaceLandmarkes = 468;

        /*
         * Face(顔)は数が多いので使う分だけ定義
         */

        /// <summary>顔：鼻先</summary>
        public const int FaceNoseTip = 1;
        /// <summary>顔：顎</summary>
        public const int FaceJaw = 152;
        /// <summary>顔：右目</summary>
        public const int FaceRightEye = 33;
        /// <summary>顔：左目</summary>
        public const int FaceLeftEye = 263;
        /// <summary>顔：右端の頬下あたり</summary>
        public const int FaceRightEdgeIndex = 132;
        /// <summary>顔：左端の頬下あたり</summary>
        public const int FaceLeftEdgeIndex = 361;

        #endregion

        #region Hand

        public const int HandLandmarkes = 20;

        /// <summary>手</summary>
        public enum Hand : int
        {
            /// <summary>手首</summary>
            Wrist = 0,

            /// <summary>親指：付根</summary>
            ThumbCmcIndex = 1,
            /// <summary>親指：第三関節</summary>
            ThumbMcpIndex = 2,
            /// <summary>親指：第二関節</summary>
            ThumbIpIndex = 3,
            /// <summary>親指：第一関節</summary>
            ThumbTipIndex = 4,

            /// <summary>人差し指：付根</summary>
            IndexFingerMcpIndex = 5,
            /// <summary>人差し指：第三関節</summary>
            IndexFingerPipIndex = 6,
            /// <summary>人差し指：第二関節</summary>
            IndexFingerDipIndex = 7,
            /// <summary>人差し指：第一関節</summary>
            IndexFingerTipIndex = 8,

            /// <summary>中指：付根</summary>
            MiddleFingerMcpIndex = 9,
            /// <summary>中指：第三関節</summary>
            MiddleFingerPipIndex = 10,
            /// <summary>中指：第二関節</summary>
            MiddleFingerDipIndex = 11,
            /// <summary>中指：第一関節</summary>
            MiddleFingerTipIndex = 12,

            /// <summary>薬指：付根</summary>
            RingFingerMcpIndex = 13,
            /// <summary>薬指：第三関節</summary>
            RingFingerPipIndex = 14,
            /// <summary>薬指：第二関節</summary>
            RingFingerDipIndex = 15,
            /// <summary>薬指：第一関節</summary>
            RingFingerTipIndex = 16,

            /// <summary>小指：付根</summary>
            PrinkyMcpIndex = 17,
            /// <summary>小指：第三関節</summary>
            PrinkyPipIndex = 18,
            /// <summary>小指：第二関節</summary>
            PrinkyDipIndex = 19,
            /// <summary>小指：第一関節</summary>
            PrinkyTipIndex = 20,
        }

        #endregion

        #region PoseBody

        public const int PoseBodyLandmarkes = 32;

        /// <summary>体</summary>
        public enum PoseBody : int
        {
            /// <summary>鼻</summary>
            Nose = 0,

            /// <summary>左目：内側</summary>
            LeftEyeInner = 1,
            /// <summary>左目：中央</summary>
            LeftEye = 2,
            /// <summary>左目：外側</summary>
            LeftEyeOuter = 3,
            
            /// <summary>右目：内側</summary>
            RightEyeInner = 4,
            /// <summary>右目：中央</summary>
            RightEye = 5,
            /// <summary>右目：外側</summary>
            RightEyeOuter = 6,

            /// <summary>左耳</summary>
            LeftEar = 7,
            /// <summary>左耳</summary>
            RightEar = 8,

            /// <summary>口：左端</summary>
            MouthLeft = 9,
            /// <summary>口：右端</summary>
            MouthRight = 10,

            /// <summary>左肩</summary>
            LeftShoulder = 11,
            /// <summary>右肩</summary>
            RightShoulder = 12,
            /// <summary>左ひじ</summary>
            LeftElbow = 13,
            /// <summary>右ひじ</summary>
            RightElbow = 14,
            /// <summary>左手首</summary>
            LeftWrist = 15,
            /// <summary>右手首</summary>
            RightWrist = 16,

            /// <summary>左手：小指</summary>
            LeftPinky = 17,
            /// <summary>右手：小指</summary>
            RightPinky = 18,
            /// <summary>左手：人差し指</summary>
            LeftIndex = 19,
            /// <summary>右手：人差し指</summary>
            RightIndex = 20,
            /// <summary>左手：親指</summary>
            LeftThumb = 21,
            /// <summary>右手：親指</summary>
            RightThumb = 22,

            /// <summary>左尻</summary>
            LeftHip = 23,
            /// <summary>右尻</summary>
            RightHip = 24,
            /// <summary>左ひざ</summary>
            LeftKnee = 25,
            /// <summary>右ひざ</summary>
            RightKnee = 26,
            /// <summary>左足首</summary>
            LeftAnkle = 27,
            /// <summary>右足首</summary>
            RightAnkle = 28,
            /// <summary>左かかと</summary>
            LeftHeel = 29,
            /// <summary>右かかと</summary>
            RightHeel = 30,
            /// <summary>左つま先</summary>
            LeftFootIndex = 31,
            /// <summary>右つま先</summary>
            RightFootIndex = 32,
        }

        #endregion
    }
}
