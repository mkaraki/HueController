# HueController
Control Philips Hue in console

## What can you control
- Switch (on|off)
- Color (hex)

## Usage

### Regist Hue Bridge
Run with `--regist` option.

## Check Light id
```
> hueController.exe list
```

### Control

#### Switch Power
```
> HueController.exe <light id> on|off
```

#### Set Color

##### By HTML Hex
```
> HueController.exe <light id> color hex <Hex with #>
```
