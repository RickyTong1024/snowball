//
//  ts.m
//  Unity-iPhone
//
//  Created by wjh on 2019/1/21.
//

#import <Foundation/Foundation.h>
@implementation ts

int getlanguage()
{
    NSUserDefaults * defaults = [NSUserDefaults standardUserDefaults];
    
    NSArray * allLanguages = [defaults objectForKey:@"AppleLanguages"];
    
    NSString * preferredLang = [allLanguages objectAtIndex:0];
    NSLog(@"hxc____%@",preferredLang);
    if([preferredLang rangeOfString :@"zh-Hans"].location == NSNotFound)
    {//当系统语言是中文或
        return 0;
    }
    else if([preferredLang rangeOfString :@"zh-Hant"].location == NSNotFound)
    {//当系统语言是中文或
        return 0;
    }
    else{ //其它语言的情况下
        return 2;
    }
}

void returnPrice(void *p)
{
    
}

void game_open_store()
{
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:@"itms://itunes.apple.com/cn/app/id967602545?mt=8"]];
}

char *platform_id()
{
    NSString* sl = @"ios";
    char* ret = nil;
    ret = (char*) malloc([sl length] + 1);
    memcpy(ret,[sl UTF8String],([sl length] + 1));
    return ret;
}

char *platform_shard()
{
    NSString* sl = @"ios";
    char* ret = nil;
    ret = (char*) malloc([sl length] + 1);
    memcpy(ret,[sl UTF8String],([sl length] + 1));
    return ret;
}
void share_init()
{
    
}

void ShareByIos(int i, void* s,void *desc,void *url,void *image)
{
    
}
@end

