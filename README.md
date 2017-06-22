# Imaging functions for c#

### Setup.

Add the following namespaces: 

> * @using System.Drawing
> * @using System.Drawing.Imaging
> * @using Framework.AssetLibrary.Imaging

```C# 
//Load the image
string imagePath = Server.MapPath("~/Images/sample.jpg");
```

```C# 
//Read image
Image image = ImageFactory.Read(imagePath);

```

The following are the available transformations.

### Method calls

```C#
//Image otimization

//Optimize for a preset
image.Optimize(ImageFormat.Jpeg, ImageQuality.UltraLow);

//Optimize for a custom value
image.Optimize(ImageFormat.Jpeg, 20);   

//Image resize
image.Resize(new Size(300, 300));

//Image resize and fill with colour
image.Resize(new Size(300, 300), Color.Transparent);

//Image resize by width
image.ResizeByWidth(300, Color.Transparent);

//Image resize by height
image.ResizeByHeight(300, Color.Transparent);

//Image Crop
image.Resize(new Size(300, 300)).Crop(new Point(100, 100), new Size(100, 100));

//Image Crop background
image.Resize(new Size(300, 300), Color.Aqua).CropBackground();

//Image grayscale filter
image.Grayscale();

//Image errosion filter
image.Resize(new Size(300, 300)).Erosion(5);

//Image errosion edge filter
image.Resize(new Size(300, 300)).Erosion(5, true, true, true, MorphologyEdgeType.Edge);

//Image errosion edge sharpen filter
image.Resize(new Size(300, 300)).Erosion(5, true, false, true, MorphologyEdgeType.EdgeSharpen);

//Image dilation filter
image.Resize(new Size(300, 300)).Dilation(5);

//Image dilation edge filter
image.Resize(new Size(300, 300)).Dilation(5, true, true, true, MorphologyEdgeType.Edge);

//Image dilation edge sharpen filter
image.Resize(new Size(300, 300)).Dilation(5, true, false, true, MorphologyEdgeType.EdgeSharpen);

//Image colour channel filter
image.Resize(new Size(300, 300)).Channel(Rgb.Green);

//Image overlay
image.Resize(new Size(300, 300)).Overlay(Brushes.Chocolate);

//Image sobel filter
image.Resize(new Size(300, 300)).Sobel();

//Image sobel overlay
image.Resize(new Size(300, 300)).Overlay(optimizedImage.Resize(new Size(300, 300)).Sobel());

//Image blur filter
image.Resize(new Size(300, 300)).Blur();

```

