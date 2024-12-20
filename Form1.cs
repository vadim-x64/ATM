using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ATM
{
    public partial class Form1 : Form
    {
        private Timer timer;
        private Timer moveIntoPanelTimer;
        private float imageTransparency = 1.0f;
        private int jumpHeight = 30;
        private int initialY;
        private bool dragging = false;
        private int dragCursorY;

        public Form1()
        {
            InitializeComponent();
            Timer blinkTimer = new Timer();
            blinkTimer.Interval = 500;
            blinkTimer.Enabled = true;
            blinkTimer.Tick += (sender, args) =>
            {
                panel3.Visible = !panel3.Visible;
            };
            InitializeForm();
            InitializeTimer();
            pictureBox2.MouseDown += new MouseEventHandler(pictureBox2_MouseDown);
            pictureBox2.MouseMove += new MouseEventHandler(pictureBox2_MouseMove);
            pictureBox2.MouseUp += new MouseEventHandler(pictureBox2_MouseUp);
            moveIntoPanelTimer = new Timer();
            moveIntoPanelTimer.Interval = 20;
            moveIntoPanelTimer.Tick += MoveIntoPanelTimer_Tick;
            Timer timeUpdateTimer = new Timer();
            timeUpdateTimer.Interval = 1000;
            timeUpdateTimer.Tick += TimeUpdateTimer_Tick;
            timeUpdateTimer.Start();
            label2.Text = DateTime.Now.ToString("HH:mm:ss");
            label3.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        private void TimeUpdateTimer_Tick(object sender, EventArgs e)
        {
            label2.Text = DateTime.Now.ToString("HH:mm:ss");
            label3.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorY = Cursor.Position.Y;
            timer.Stop(); 
        }

        private void MoveIntoPanelTimer_Tick(object sender, EventArgs e)
        {
            imageTransparency -= 0.05f;
            pictureBox2.Top -= 2;
            if (imageTransparency < 0)
            {
                imageTransparency = 0;
                moveIntoPanelTimer.Stop();
                OpenForm2();
            }
            UpdatePictureBoxTransparency(pictureBox2, imageTransparency);
        }

        private void UpdatePictureBoxTransparency(PictureBox pictureBox, float transparency)
        {
            if (pictureBox.Image != null)
            {
                Bitmap bitmap = new Bitmap(pictureBox.Image.Width, pictureBox.Image.Height);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    ColorMatrix colorMatrix = new ColorMatrix
                    {
                        Matrix33 = transparency
                    };
                    ImageAttributes imageAttributes = new ImageAttributes();
                    imageAttributes.SetColorMatrix(colorMatrix);
                    g.DrawImage(pictureBox.Image, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, pictureBox.Image.Width, pictureBox.Image.Height, GraphicsUnit.Pixel, imageAttributes);
                }
                pictureBox.Image = bitmap;
            }
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                int newY = pictureBox2.Location.Y + (Cursor.Position.Y - dragCursorY);
                dragCursorY = Cursor.Position.Y;
                int bottomMargin = 10; 
                newY = Math.Min(newY, this.ClientSize.Height - pictureBox2.Height - bottomMargin);
                pictureBox2.Location = new Point(pictureBox2.Location.X, newY);
                if (pictureBox2.Bounds.IntersectsWith(panel5.Bounds))
                {
                    dragging = false;
                    timer.Stop();  
                }
            }
        }

        private void OpenForm2()
        {
            this.Hide();
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
            if (pictureBox2.Bounds.IntersectsWith(panel5.Bounds))
            {
                moveIntoPanelTimer.Start();
            }
            else
            {
                timer.Start();
            }
        }

        private void InitializeForm()
        {
            initialY = pictureBox2.Location.Y;
        }

        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 20;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            pictureBox2.Location = new Point(pictureBox2.Location.X, initialY + (int)(jumpHeight * Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 200)));
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Application.Exit();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Hide();
            AdminForm_8_ adminForm_8_ = new AdminForm_8_();
            adminForm_8_.ShowDialog();
        }
    }
}