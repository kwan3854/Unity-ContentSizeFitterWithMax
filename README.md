# ContentSizeFitterWithMax

A custom Unity UI component that extends ContentSizeFitter with maximum size constraints, allowing for more flexible UI layouts that can adapt to different screen sizes and resolutions.

## Features

- All functionality of the standard Unity ContentSizeFitter
- Maximum width and height constraints
- Three sizing options for maximum constraints:
  - **Pixel**: Fixed size in pixels
  - **ScreenPercentage**: Percentage of the screen size
  - **CanvasPercentage**: Percentage of the parent Canvas size
- Automatically adjusts when screen size or Canvas size changes
- Works seamlessly with Unity's layout system
- Performance optimized for both editor and runtime environments

## Installation

1. Copy the `ContentSizeFitterWithMax` folder into your Unity project's Assets folder
2. Add the component to any UI element that already has a RectTransform

## Usage

1. Add the ContentSizeFitterWithMax component to a UI GameObject:
   - Component -> Layout -> Content Size Fitter With Max

2. Configure the standard ContentSizeFitter settings:
   - Horizontal Fit: Unconstrained, Min Size, or Preferred Size
   - Vertical Fit: Unconstrained, Min Size, or Preferred Size

3. Set maximum size constraints:
   - **Width Type**: Select Pixel, ScreenPercentage, or CanvasPercentage
   - **Max Width**: Set the maximum width value
   - **Height Type**: Select Pixel, ScreenPercentage, or CanvasPercentage
   - **Max Height**: Set the maximum height value

4. Configure update behavior (optional):
   - **Dynamic Update Mode**: Choose when the component recalculates sizes
     - OnlyWhenChanged: Updates only when values are explicitly changed (best performance)
     - OnResolutionChange: Updates when screen resolution changes (recommended)
     - Continuous: Updates every frame (most responsive, higher performance cost)

## Examples

### Fixed Maximum Size
Set Width Type and Height Type to "Pixel" with specific values to ensure content never exceeds those dimensions.

### Responsive UI
Set Width Type to "CanvasPercentage" with value 80 to make the element take up to 80% of the canvas width, but still respecting the content size fitting behavior.

### Screen-Adaptive Elements
Set Height Type to "ScreenPercentage" with value 50 to make sure your element never takes more than half the screen height.

### Mobile-Friendly UI
For mobile games with orientation changes, set update mode to "OnResolutionChange" to automatically adapt when the device rotates.

## Performance Notes

- In Editor: Updates continuously to provide real-time feedback during design
- In Runtime: By default, updates only when values change or on resolution changes
- For elements that need continuous updates (rare), you can enable "Continuous" mode
- Setting a max width/height to -1 or 0 disables that constraint
- For optimal performance, only use CanvasPercentage when necessary
- The component uses Unity's Screen.onScreenDimensionsChanged event for efficient resolution change detection

## License

This component is released under the MIT License.