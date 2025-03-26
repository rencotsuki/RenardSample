#import <AudioToolbox/AudioServices.h>

extern "C"
{
    /* バッテリー残量取得 */
    float BatteryLevelNative() 
    {
        [UIDevice currentDevice].batteryMonitoringEnabled = YES;
        float batteryLevel = [UIDevice currentDevice].batteryLevel;
        return batteryLevel;
    }

    /* バッテリー状態取得 */
    int BatteryStatusNative() 
    {
        [UIDevice currentDevice].batteryMonitoringEnabled = YES;
        if ([UIDevice currentDevice].batteryState == UIDeviceBatteryStateUnknown) return -1;
        if ([UIDevice currentDevice].batteryState == UIDeviceBatteryStateUnplugged) return 0;
        if ([UIDevice currentDevice].batteryState == UIDeviceBatteryStateCharging) return 1;
        if ([UIDevice currentDevice].batteryState == UIDeviceBatteryStateFull) return 2;
        return -1;
    }

    /* デバイス発熱状態取得 */
    int ThermalStateNative()
    {
        return (int) [NSProcessInfo.processInfo thermalState];
    }

    /* 録画状態取得 */
    bool GetCaptured()
    {
        if ([UIScreen mainScreen].isCaptured == YES)
            return true;
        return false;
    }

    /* ストレージサイズ取得(MB) ※0未満は取得失敗 */
    float GetStorageFreeSize()
    {
        NSArray *paths = NSSearchPathForDirectoriesInDomains(NSLibraryDirectory, NSUserDomainMask, YES);
        NSDictionary *dic = [[NSFileManager defaultManager] attributesOfFileSystemForPath:[paths lastObject] error:nil];
        if (dic)
            return [dic[@"NSFileSystemFreeSize"] floatValue] / (1024.0 * 1024.0);
        return -1;
    }

    /* システムサウンド（バイブレーション）再生 */
    void PlaySystemSound (int soundId)
    {
        AudioServicesPlaySystemSound(soundId);
    }
}
