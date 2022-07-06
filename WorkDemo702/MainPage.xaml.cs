using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Composition.Interactions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace WorkDemo702
{
    public sealed partial class MainPage : Page
    {
        private Compositor _compositor;
        private Visual _imageVisual;
        private bool _isState1;
        private CompositionPropertySet _propertySet;
        private InteractionTracker _tracker;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void GoToState1()
        {
            _tracker.TryUpdatePosition(new Vector3(0, _tracker.MinPosition.Y, 0));

            _isState1 = true;
        }

        private void GoToState2()
        {
            _tracker.TryUpdatePosition(new Vector3(0, _tracker.MaxPosition.Y, 0));

            _isState1 = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _imageVisual = ElementCompositionPreview.GetElementVisual(Image);
            _imageVisual.Size = new Vector2((float)Image.ActualWidth, (float)Image.ActualHeight);
            _imageVisual.CenterPoint = new Vector3((float)Image.ActualWidth / 2.0f, (float)Image.ActualHeight / 2.0f, 0);

            _compositor = _imageVisual.Compositor;

            _propertySet = _compositor.CreatePropertySet();
            _propertySet.InsertScalar("progress", 0.0f);

            _tracker = InteractionTracker.Create(_compositor);
            _tracker.MaxPosition = new Vector3(0, _imageVisual.Size.Y * 0.5f, 0);
            _tracker.MinPosition = new Vector3();

            var trackerNode = _tracker.GetReference();
            var progressExp = ExpressionFunctions.Clamp(trackerNode.Position.Y / trackerNode.MaxPosition.Y, 0, 1);
            _propertySet.StartAnimation("progress", progressExp);

            var startNode = ExpressionValues.Constant.CreateConstantVector3("start");
            var endNode = ExpressionValues.Constant.CreateConstantVector3("end");
            var progress = _propertySet.GetReference().GetScalarProperty("progress");
            var expression = ExpressionFunctions.Lerp(startNode, endNode, progress);
            expression.SetVector3Parameter("start", new Vector3(1.0f, 1.0f, 1.0f));
            expression.SetVector3Parameter("end", new Vector3(1.1f, 1.1f, 1.1f));
            _imageVisual.StartAnimation("scale", expression);

            GoToState1();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isState1)
            {
                GoToState2();
            }
            else
            {
                GoToState1();
            }
        }
    }
}