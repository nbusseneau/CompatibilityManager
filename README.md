# Compatibility Manager

A Windows app for bulk managing compatibility flags. It allows you to quickly enable/disable compatibility settings for multiple executables at once, instead of having to go through them one by one.

## But why?

Because in Windows 10 Fall Creators Update (1709), there is no possibility to globally "disable fullscreen optimizations"™.

There used to be one in [Anniversary Update (1607)](https://www.reddit.com/r/Windows10/comments/645ukf/windows_10_cu_fullscreen_optimizations/dhounib/?context=100).
Which was still working in [Creators Update (1703)](https://www.reddit.com/r/Windows10/comments/645ukf/windows_10_cu_fullscreen_optimizations/dmyx1y9/?context=100).
But now [it ain't working no more](https://www.reddit.com/r/Windows10/comments/78r88x/can_i_disable_full_screen_optimizations_globally/dp95fpy/?context=100).

## But aren't "fullscreen optimizations"™ good?

Maybe! If they're working for you, that's great: you should keep using them.

In my case, they don't seem to work as intended, and introduce stuttering. I have confirmed "fullscreen optimizations"™ to be the cause of these issues because either:
* Switching a game from fullscreen to windowed or borderless (where "fullscreen optimizations"™ can't do no harm) makes the stuttering disappear.
* Disabling "fullscreen optimizations"™ makes the stuttering go away.

So, I'd just like to be able to disable them once and for all, Microsoft please pretty please :((

## Why isn't it called "Fullscreen Optimizations"™ Manager then?

Because it does a bit more than that. Please don't assume Compatibility Manager's intended behaviour, that triggers me.