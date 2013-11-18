using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Gimela.Presentation.Transitions
{
    /// <summary>
    /// 3D过渡的基类
    /// </summary>
    public abstract class Transition3D : Transition
    {
        static Transition3D()
        {
            Model3DGroup defaultLight = new Model3DGroup();

            Vector3D direction = new Vector3D(1, 1, 1);
            direction.Normalize();

            //周围环境
            byte ambient = 108; // 108 is minimum for directional to be < 256 (for direction = [1,1,1])
            //方向
            byte directional = (byte)Math.Min((255 - ambient) / Vector3D.DotProduct(direction, new Vector3D(0, 0, 1)), 255);

            defaultLight.Children.Add(new AmbientLight(Color.FromRgb(ambient, ambient, ambient)));
            defaultLight.Children.Add(new DirectionalLight(Color.FromRgb(directional, directional, directional), direction));
            defaultLight.Freeze();

            LightProperty = DependencyProperty.Register("Light", 
                typeof(Model3D), typeof(Transition3D), new UIPropertyMetadata(defaultLight));
        }

        public double FieldOfView
        {
            get { return (double)GetValue(FieldOfViewProperty); }
            set { SetValue(FieldOfViewProperty, value); }
        }

        public static readonly DependencyProperty FieldOfViewProperty = DependencyProperty.Register("FieldOfView", typeof(double), typeof(Transition3D), new UIPropertyMetadata(20.0));

        public Model3D Light
        {
            get { return (Model3D)GetValue(LightProperty); }
            set { SetValue(LightProperty, value); }
        }

        public static readonly DependencyProperty LightProperty;

        /// <summary>
        /// Setup the Viewport 3D
        /// </summary>
        /// <param name="transitionElement"></param>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected internal sealed override void BeginTransition(TransitionElement transitionElement, ContentPresenter oldContent, ContentPresenter newContent)
        {
            Viewport3D viewport = new Viewport3D();
            viewport.IsHitTestVisible = false;

            viewport.Camera = CreateCamera(transitionElement, FieldOfView);
            viewport.ClipToBounds = false;
            ModelVisual3D light = new ModelVisual3D();
            light.Content = Light;
            viewport.Children.Add(light);

            transitionElement.Children.Add(viewport);

            BeginTransition3D(transitionElement, oldContent, newContent, viewport);
        }

        protected virtual Camera CreateCamera(TransitionElement transitionElement, double fov)
        {
            Size size = transitionElement.RenderSize;
            return new PerspectiveCamera(new Point3D(size.Width / 2, size.Height / 2, -size.Width / Math.Tan(fov / 2 * Math.PI / 180) / 2),
                                         new Vector3D(0, 0, 1),
                                         new Vector3D(0, -1, 0),
                                         fov);
        }

        protected virtual void BeginTransition3D(TransitionElement transitionElement, ContentPresenter oldContent, ContentPresenter newContent, Viewport3D viewport)
        {
            EndTransition(transitionElement, oldContent, newContent);
        }

        /// <summary>
        /// Generates a flat mesh starting at origin with sides equal to u and v vecotrs
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="usteps"></param>
        /// <param name="vsteps"></param>
        /// <param name="textureBounds"></param>
        /// <returns></returns>
        public static MeshGeometry3D CreateMesh(Point3D origin, Vector3D u, Vector3D v, int usteps, int vsteps, Rect textureBounds)
        {
            u = 1.0 / usteps * u;
            v = 1.0 / vsteps * v;

            MeshGeometry3D mesh = new MeshGeometry3D();

            for (int i = 0; i <= usteps; i++)
            {
                for (int j = 0; j <= vsteps; j++)
                {
                    mesh.Positions.Add(origin + i * u + j * v);

                    mesh.TextureCoordinates.Add(new Point(textureBounds.X + textureBounds.Width * i / usteps,
                                                          textureBounds.Y + textureBounds.Height * j / vsteps));

                    if (i > 0 && j > 0)
                    {
                        mesh.TriangleIndices.Add((i - 1) * (vsteps + 1) + (j - 1));
                        mesh.TriangleIndices.Add((i - 0) * (vsteps + 1) + (j - 0));
                        mesh.TriangleIndices.Add((i - 0) * (vsteps + 1) + (j - 1));

                        mesh.TriangleIndices.Add((i - 1) * (vsteps + 1) + (j - 1));
                        mesh.TriangleIndices.Add((i - 1) * (vsteps + 1) + (j - 0));
                        mesh.TriangleIndices.Add((i - 0) * (vsteps + 1) + (j - 0));
                    }
                }
            }
            return mesh;
        }
    }
}
