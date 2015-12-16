using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsGL.OpenGL;

namespace SerialTunningTool
{
    public partial class View3D : Form
    {
        private GLView glView = null;
        private static Double side = 50;
        private static Double[] proj = {side, side, side};
        private Double length = Math.Sqrt(proj[0] * proj[0] + proj[1] * proj[1] + proj[2] * proj[2]);

        public View3D(DataAdapter[] adapter)
        {
            InitializeComponent();
            Activated += View3D_Activated;
            FormClosed += View3D_FormClosed;
            glView = new GLView(adapter);
            glView.Dock = DockStyle.Fill;
            glView.Location = new Point(0, 0);
            glView.Name = "GLView";
            glView.Visible = true;
            Controls.Add(glView);
            ResumeLayout(false);
            glView.Initialization();
        }

        void View3D_FormClosed(object sender, FormClosedEventArgs e)
        {
            glView.Release();
        }

        void View3D_Activated(object sender, EventArgs e)
        {
            TopLevel = true;
        }

        public class GLView : OpenGLControl {
            private Timer timer = null;
            private DataAdapter[] View3DAdapter = null;

            public GLView(DataAdapter[] adapter) {
                View3DAdapter = adapter;
                timer = new Timer();
                timer.Interval = 50;
                timer.Tick += timer_Tick;
                timer.Start();
            }

            void timer_Tick(object sender, EventArgs e)
            {               
                Invalidate();
            }

            public void Release() {
                timer.Stop();
                GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_ACCUM_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);
                GL.glFinish();
            }

            public void Initialization() {
                GL.glViewport(0, 0, Size.Width, Size.Height);
                GL.glMatrixMode(GL.GL_PROJECTION);
                GL.glLoadIdentity();
                GL.glOrtho(0.0, Size.Width, 0.0, Size.Height, -1000.0, 1000.0);
                GL.glMatrixMode(GL.GL_MODELVIEW);
                GL.glLoadIdentity();
            }

            protected override void OnPaint(PaintEventArgs pevent) {
                try
                {
                    base.OnPaint(pevent);
                }
                catch{}
            }

            protected override void OnSizeChanged(EventArgs e)
            {
                base.OnSizeChanged(e);
                Initialization();
            }

            public override void glDraw()
            {
                GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_ACCUM_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);
                GL.glPushMatrix();
                GL.glLoadIdentity();
                GL.glTranslatef(Size.Width * 3 / 4, Size.Height / 2, 0.0f);
                GL.gluLookAt(0.0, 0.0, 100.0, Size.Width / 2, Size.Height / 2, 0.0, 0.0, 1.0, 0.0);
                
                GLUquadric quad = GL.gluNewQuadric();
                GL.gluQuadricDrawStyle(quad, GL.GLU_FILL);
                GL.gluQuadricNormals(quad, GL.GLU_SMOOTH);

                GL.glColor3f(1.0f, 0.0f, 0.0f);
                GL.gluCylinder(quad, 1.0, 1.0, 200.0, 32, 32);
                GL.glRotated(180.0, 0, 1, 0);
                GL.gluCylinder(quad, 1.0, 1.0, 200.0, 32, 32);
                GL.glTranslatef(0.0f, 0.0f, 200.0f);
                GL.gluCylinder(quad, 5.0, 0.0, 20.0, 32, 32);
                GL.glTranslatef(0.0f, 0.0f, -200.0f);

                GL.glColor3f(0.0f, 1.0f, 0.0f);
                GL.glRotated(90.0, 0, 1, 0);
                GL.gluCylinder(quad, 1.0, 1.0, 200.0, 32, 32);
                GL.glRotated(-180.0, 0, 1, 0);
                GL.gluCylinder(quad, 1.0, 1.0, 200.0, 32, 32);
                GL.glTranslatef(0.0f, 0.0f, 200.0f);
                GL.gluCylinder(quad, 5.0, 0.0, 20.0, 32, 32);
                GL.glTranslatef(0.0f, 0.0f, -200.0f);

                GL.glColor3f(0.0f, 0.0f, 1.0f);
                GL.glRotated(90.0, 1, 0, 0);
                GL.gluCylinder(quad, 1.0, 1.0, 200.0, 32, 32);
                GL.glRotated(-180.0, 1, 0, 0);
                GL.gluCylinder(quad, 1.0, 1.0, 200.0, 32, 32);
                GL.glTranslatef(0.0f, 0.0f, 200.0f);
                GL.gluCylinder(quad, 5.0, 0.0, 20.0, 32, 32);
                GL.glTranslatef(0.0f, 0.0f, -200.0f);

                if (View3DAdapter[0].getData().Length > 0)
                {
                    GL.glRotated(-Convert.ToDouble(View3DAdapter[0].getData()[View3DAdapter[0].getData().Length - 1]), 1, 0, 0);
                }

                if (View3DAdapter[1].getData().Length > 0)
                {
                    GL.glRotated(Convert.ToDouble(View3DAdapter[1].getData()[View3DAdapter[1].getData().Length - 1]), 0, 1, 0);
                }

                if (View3DAdapter[2].getData().Length > 0)
                {
                    GL.glRotated(-Convert.ToDouble(View3DAdapter[2].getData()[View3DAdapter[2].getData().Length - 1]), 0, 0, 1);
                }                

                GL.glColor3f(1.0f, 1.0f, 1.0f);
                GL.gluCylinder(quad, 5.0, 5.0, 100.0, 32, 32);
                GL.glTranslatef(0.0f, 0.0f, 100.0f);
                GL.gluCylinder(quad, 10.0, 0.0, 40.0, 32, 32);
                GL.glTranslatef(0.0f, 0.0f, -100.0f);

                GL.gluDeleteQuadric(quad); 

                GL.glPopMatrix();
                for (int i = 0; i < 3; i++ )
                {
                    if (View3DAdapter[i].getData().Length > 10)
                    {
                        View3DAdapter[i].clear();
                    }
                }
            }
        }
    }
}
