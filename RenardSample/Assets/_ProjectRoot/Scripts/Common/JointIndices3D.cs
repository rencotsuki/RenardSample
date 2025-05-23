﻿using System;

public enum JointIndices3D
{
    Invalid = -1,
    Root = 0, // parent: <none> [-1]
    Hips = 1, // parent: Root [0]
    LeftUpLeg = 2, // parent: Hips [1]
    LeftLeg = 3, // parent: LeftUpLeg [2]
    LeftFoot = 4, // parent: LeftLeg [3]
    LeftToes = 5, // parent: LeftFoot [4]
    LeftToesEnd = 6, // parent: LeftToes [5]
    RightUpLeg = 7, // parent: Hips [1]
    RightLeg = 8, // parent: RightUpLeg [7]
    RightFoot = 9, // parent: RightLeg [8]
    RightToes = 10, // parent: RightFoot [9]
    RightToesEnd = 11, // parent: RightToes [10]
    Spine1 = 12, // parent: Hips [1]
    Spine2 = 13, // parent: Spine1 [12]
    Spine3 = 14, // parent: Spine2 [13]
    Spine4 = 15, // parent: Spine3 [14]
    Spine5 = 16, // parent: Spine4 [15]
    Spine6 = 17, // parent: Spine5 [16]
    Spine7 = 18, // parent: Spine6 [17]
    LeftShoulder1 = 19, // parent: Spine7 [18]
    LeftArm = 20, // parent: LeftShoulder1 [19]
    LeftForearm = 21, // parent: LeftArm [20]
    LeftHand = 22, // parent: LeftForearm [21]
    LeftHandIndexStart = 23, // parent: LeftHand [22]
    LeftHandIndex1 = 24, // parent: LeftHandIndexStart [23]
    LeftHandIndex2 = 25, // parent: LeftHandIndex1 [24]
    LeftHandIndex3 = 26, // parent: LeftHandIndex2 [25]
    LeftHandIndexEnd = 27, // parent: LeftHandIndex3 [26]
    LeftHandMidStart = 28, // parent: LeftHand [22]
    LeftHandMid1 = 29, // parent: LeftHandMidStart [28]
    LeftHandMid2 = 30, // parent: LeftHandMid1 [29]
    LeftHandMid3 = 31, // parent: LeftHandMid2 [30]
    LeftHandMidEnd = 32, // parent: LeftHandMid3 [31]
    LeftHandPinkyStart = 33, // parent: LeftHand [22]
    LeftHandPinky1 = 34, // parent: LeftHandPinkyStart [33]
    LeftHandPinky2 = 35, // parent: LeftHandPinky1 [34]
    LeftHandPinky3 = 36, // parent: LeftHandPinky2 [35]
    LeftHandPinkyEnd = 37, // parent: LeftHandPinky3 [36]
    LeftHandRingStart = 38, // parent: LeftHand [22]
    LeftHandRing1 = 39, // parent: LeftHandRingStart [38]
    LeftHandRing2 = 40, // parent: LeftHandRing1 [39]
    LeftHandRing3 = 41, // parent: LeftHandRing2 [40]
    LeftHandRingEnd = 42, // parent: LeftHandRing3 [41]
    LeftHandThumbStart = 43, // parent: LeftHand [22]
    LeftHandThumb1 = 44, // parent: LeftHandThumbStart [43]
    LeftHandThumb2 = 45, // parent: LeftHandThumb1 [44]
    LeftHandThumbEnd = 46, // parent: LeftHandThumb2 [45]
    Neck1 = 47, // parent: Spine7 [18]
    Neck2 = 48, // parent: Neck1 [47]
    Neck3 = 49, // parent: Neck2 [48]
    Neck4 = 50, // parent: Neck3 [49]
    Head = 51, // parent: Neck4 [50]
    Jaw = 52, // parent: Head [51]
    Chin = 53, // parent: Jaw [52]
    LeftEye = 54, // parent: Head [51]
    LeftEyeLowerLid = 55, // parent: LeftEye [54]
    LeftEyeUpperLid = 56, // parent: LeftEye [54]
    LeftEyeball = 57, // parent: LeftEye [54]
    Nose = 58, // parent: Head [51]
    RightEye = 59, // parent: Head [51]
    RightEyeLowerLid = 60, // parent: RightEye [59]
    RightEyeUpperLid = 61, // parent: RightEye [59]
    RightEyeball = 62, // parent: RightEye [59]
    RightShoulder1 = 63, // parent: Spine7 [18]
    RightArm = 64, // parent: RightShoulder1 [63]
    RightForearm = 65, // parent: RightArm [64]
    RightHand = 66, // parent: RightForearm [65]
    RightHandIndexStart = 67, // parent: RightHand [66]
    RightHandIndex1 = 68, // parent: RightHandIndexStart [67]
    RightHandIndex2 = 69, // parent: RightHandIndex1 [68]
    RightHandIndex3 = 70, // parent: RightHandIndex2 [69]
    RightHandIndexEnd = 71, // parent: RightHandIndex3 [70]
    RightHandMidStart = 72, // parent: RightHand [66]
    RightHandMid1 = 73, // parent: RightHandMidStart [72]
    RightHandMid2 = 74, // parent: RightHandMid1 [73]
    RightHandMid3 = 75, // parent: RightHandMid2 [74]
    RightHandMidEnd = 76, // parent: RightHandMid3 [75]
    RightHandPinkyStart = 77, // parent: RightHand [66]
    RightHandPinky1 = 78, // parent: RightHandPinkyStart [77]
    RightHandPinky2 = 79, // parent: RightHandPinky1 [78]
    RightHandPinky3 = 80, // parent: RightHandPinky2 [79]
    RightHandPinkyEnd = 81, // parent: RightHandPinky3 [80]
    RightHandRingStart = 82, // parent: RightHand [66]
    RightHandRing1 = 83, // parent: RightHandRingStart [82]
    RightHandRing2 = 84, // parent: RightHandRing1 [83]
    RightHandRing3 = 85, // parent: RightHandRing2 [84]
    RightHandRingEnd = 86, // parent: RightHandRing3 [85]
    RightHandThumbStart = 87, // parent: RightHand [66]
    RightHandThumb1 = 88, // parent: RightHandThumbStart [87]
    RightHandThumb2 = 89, // parent: RightHandThumb1 [88]
    RightHandThumbEnd = 90, // parent: RightHandThumb2 [89]
}

public enum JointIndices2D
{
    Invalid = -1,
    Head = 0, // parent: Neck1 [1]
    Neck1 = 1, // parent: Root [16]
    RightShoulder1 = 2, // parent: Neck1 [1]
    RightForearm = 3, // parent: RightShoulder1 [2]
    RightHand = 4, // parent: RightForearm [3]
    LeftShoulder1 = 5, // parent: Neck1 [1]
    LeftForearm = 6, // parent: LeftShoulder1 [5]
    LeftHand = 7, // parent: LeftForearm [6]
    RightUpLeg = 8, // parent: Root [16]
    RightLeg = 9, // parent: RightUpLeg [8]
    RightFoot = 10, // parent: RightLeg [9]
    LeftUpLeg = 11, // parent: Root [16]
    LeftLeg = 12, // parent: LeftUpLeg [11]
    LeftFoot = 13, // parent: LeftLeg [12]
    RightEye = 14, // parent: Head [0]
    LeftEye = 15, // parent: Head [0]
    Root = 16, // parent: <none> [-1]
    RightEar = 17, // parent: RightEye [14] (iOS 16 and up)
    LeftEar = 18 // parent: LeftEye [15] (iOS 16 and up)
}

public static class JointIndicesExtension
{
    public static int NumSkeletonJoints => 91;

    public static int Joint3DNameToIndex(string jointName)
    {
        try
        {
            JointIndices3D val;

            if (!Enum.TryParse(jointName, out val))
                throw new Exception("parse error.");

            return (int)val;
        }
        catch
        {
            return -1;
        }
    }

    public static int Joint2DNameToIndex(string jointName)
    {
        try
        {
            JointIndices2D val;

            if (!Enum.TryParse(jointName, out val))
                throw new Exception("parse error.");

            return (int)val;
        }
        catch
        {
            return -1;
        }
    }
}
