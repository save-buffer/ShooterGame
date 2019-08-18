using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Engine
{
    public enum CollisionTypes //The types of collision that the GameObject can have
    {
        None,
        Circular,
        Rectangular
    }
    class GameObject : Element
    {
        string Texture; //The filename of the model
        protected uint Width;
        protected uint Height;

        public bool IsInViewport //Checks if the GameObject is within the bounds of the Game
        {
            get
            {
                return !(Position.X + Width / 2 > Game.Dimensions.Width / 2 ||
                    Position.Y + Height / 2 > Game.Dimensions.Height / 2 ||
                    Position.X - Width / 2 < -Game.Dimensions.Width / 2 ||
                    Position.Y - Height / 2 < -Game.Dimensions.Height / 2);
            }
        }
        public bool IsDead
        {
            set
            {
                if (value)
                {
                    OnDeath();
                    this.Dispose();
                }
            }
        }
        public bool ScreenWrapping = false; //Tells it whether or not to screenwrap
        #region Collision
        public CollisionTypes Collision = CollisionTypes.None;
        public int CollisionRadius = 0;
        public Engine.PointF CollisionBox;
        #endregion
        #region Scale
        private double scalex, scaley;
        private List<Frame> Originals;
        private uint original_width, original_height;
        public double ScaleX
        {
            get
            {
                return scalex;
            }
            set
            {
                Width = (uint)(original_width * value);
                try
                {
                    for (int i = 0; i < Originals.Count; i++)
                    {
                        Models[i].Image = ImageHelper.ResizeImage(Originals[i].Image, (int)Width, (int)Height);
                    }
                    Model = Models[(int)CurrentModel].Image;
                    scalex = value;
                }
                catch
                {
                    Width = (uint)(Width / value);
                }
            }
        }
        public double ScaleY
        {
            get
            {
                return scaley;
            }
            set
            {
                Height = (uint)(original_height * value);
                try
                {
                    for (int i = 0; i < Originals.Count; i++)
                    {
                        Models[i].Image = ImageHelper.ResizeImage(Originals[i].Image, (int)Width, (int)Height);
                    }
                    Model = Models[(int)CurrentModel].Image;
                    scaley = value;
                }
                catch
                {
                    Height = (uint)(Height / value);
                }
            }
        }
        #endregion
        #region DrawingValues
        public const int BACKGROUND = -1;
        public const int MIDGROUND = 0;
        public const int FOREGROUND = 1;

        public float Rotation = 0; //Rotates the image
        #endregion
        #region Basic
        public GameObject(string Name, string Texture, uint Width, uint Height) : base(Name)
        {
            this.Texture = Texture;
            this.Height = Height;
            this.Width = Width;
            this.original_width = (uint)Width;
            this.original_height = (uint)Height;

            this.Position = new PointF(0, 0); //Initialize it to default position
            ZOrder = MIDGROUND; //The default ZOrder is 0 (MIDGROUND)
            Models = new List<Frame>(); //The list of Frames that the object has
            Originals = new List<Frame>();

            try
            {
                Image i = Image.FromFile("Assets\\" + Texture); //Load the image
                Models.Add(new Frame(i, 0)); //Add the first image into the Frame
                Originals.Add(new Frame(i, 0));
                Model = i; //Set the current model to the default texture
            }
            catch
            {
                Models.Add(null); //Otherwise add a black box
                Originals.Add(null);
                Model = null;
            }
            scalex = 1;
            scaley = 1;
        }
        public override void Draw(Graphics G, PointF Dimensions)
        {
            base.Draw(G, Dimensions);
            if (Model != null)
            {
                PointF ScreenPosition = new PointF((Position.X + Dimensions.X / 2) - Width / 2,
                    (-Position.Y + Dimensions.Y / 2) - Height / 2); //Convert Cartesian Coordinates to Screen Coordinates

                G.TranslateTransform(ScreenPosition.X + Width / 2, ScreenPosition.Y + Height / 2);
                G.RotateTransform(-Rotation);
                G.TranslateTransform(-ScreenPosition.X - Width / 2, -ScreenPosition.Y - Height / 2); //Apply Rotation
                G.DrawImage(Model, ScreenPosition); //Draw the image

                G.TranslateTransform(ScreenPosition.X + Width / 2, ScreenPosition.Y + Height / 2); //Rotate the Graphics context back
                G.RotateTransform(Rotation);
                G.TranslateTransform(-ScreenPosition.X - Width / 2, -ScreenPosition.Y - Height / 2);
            }
        }
        public override void Update()
        {
            base.Update();
            ScreenWrap();
            if (AnimationTimer == 0 && AnimationDirection != 0)
            {
                NextFrame();
            }
            AnimationTimer--;
        }
        public virtual void ScreenWrap()
        {
            if (ScreenWrapping) //If we want to screenwrap, then screenwrap!
            {
                if (Position.X > Game.Dimensions.Width / 2)
                    Position.X = -Game.Dimensions.Width / 2;
                if (Position.Y > Game.Dimensions.Height / 2)
                    Position.Y = -Game.Dimensions.Height / 2;
                if (Position.X < -Game.Dimensions.Width / 2)
                    Position.X = Game.Dimensions.Width / 2;
                if (Position.Y < -Game.Dimensions.Height / 2)
                    Position.Y = Game.Dimensions.Height / 2;
            }
        }
        public virtual void OnDeath() //A function that is possible to override in case the user wants to do something when the GameObject dies.
        {

        }
        public void Dispose()
        {
            Game.RemoveElement(this.Name);
        }
        public GameObject Clone()
        {
            return (GameObject)this.MemberwiseClone();
        }
        #endregion
        #region Animation
        protected List<Frame> Models; //This is the list of all of the models
        protected Image Model; //This is the current model

        private int AnimationDirection; //Is set to 1 if it is animating forward and -1 if it is animating backwards
        private int AnimationTimer; //Keeps track of the duration of each frame.
        protected int CurrentModel; //The index of the current image in the list

        public void AnimateForwards() //Start animating forwards
        {
            AnimationDirection = 1;
        }
        public void AnimateBackwards() //Start animating backwards
        {
            AnimationDirection = -1;
        }
        public void StopAnimation() //Stop Animating
        {
            AnimationDirection = 0;
        }
        protected class Frame //This holds an image and a duration for the frame
        {
            public Image Image;
            public int Duration;
            public Frame(Image Model, int Duration)
            {
                this.Image = Model;
                this.Duration = Duration;
            }
        }
        public void SetFrameDuration(uint Frame, uint Duration) //Sets the duration of the specified frame
        {
            Models[(int)Frame].Duration = (int)Duration;
        }
        public void SetFrameModel(uint Frame, string Filename) //Sets the model of the specified frame
        {
            Models[(int)Frame].Image = Image.FromFile("Assets\\" + Filename);
        }
        public void SetFrameModel(uint Frame, Image i)
        {
            Models[(int)Frame].Image = i;
        }
        public void FlipFrame(uint Frame, RotateFlipType f)
        {
            Models[(int)Frame].Image.RotateFlip(f);
        }
        public void AddFrame(string Filename, uint Duration) //Adds a new frame
        {
            Image i = Image.FromFile("Assets\\" + Filename);
            Models.Add(new Frame(i, (int)Duration));
            Originals.Add(new Frame(i, (int)Duration));
        }
        public void AddFrame(Image i, uint Duration) //Adds a new frame
        {
            Models.Add(new Frame(i, (int)Duration));
            Originals.Add(new Frame(i, (int)Duration));
        }
        public void RemoveFrame(uint Frame) //Removes a frame
        {
            Models.RemoveAt((int)Frame);
            Originals.RemoveAt((int)Frame);
        }
        public void ClearFrames()
        {
            Models.Clear();
            Originals.Clear();
            this.CurrentModel = 0;
        }
        public uint NumberOfFrames()
        {
            return (uint)this.Models.Count;
        }
        public void NextFrame() //Goes to the next frame
        {
            CurrentModel += AnimationDirection;
            if (CurrentModel <= -1)
                CurrentModel = Models.Count - 1;
            else if (CurrentModel >= Models.Count)
                CurrentModel = 0;

            Model = Models[CurrentModel].Image;
            AnimationTimer = Models[CurrentModel].Duration;
        }
        public void PreviousFrame() //Goes to the previous frame
        {
            CurrentModel -= AnimationDirection;
            if (CurrentModel <= -1)
                CurrentModel = Models.Count - 1;
            else if (CurrentModel >= Models.Count)
                CurrentModel = 0;

            Model = Models[CurrentModel].Image;
            AnimationTimer = Models[CurrentModel].Duration;
        }
        public void GoToFrame(uint Frame) //Jumps to the specified frame
        {
            if (Frame < Models.Count)
            {
                CurrentModel = (int)Frame;
                Model = Models[CurrentModel].Image;
                AnimationTimer = Models[CurrentModel].Duration;
            }
        }
        #endregion
        #region CollisionFunctions
        public bool IsCollidedWithGameObject(GameObject Obj) //Checks if this GameObject is collided with another one.
        {
            if (Collision == CollisionTypes.None || Obj.Collision == CollisionTypes.None) //If Collision is off, they're not colliding
            {
                return false;
            }
            else if (Collision == CollisionTypes.Circular)
            {
                if (Obj.Collision == CollisionTypes.Circular) //In the case that they both have circular collision 
                {
                    if (Distance(Position, Obj.Position) < CollisionRadius || Distance(Position, Obj.Position) < Obj.CollisionRadius)
                        return true;
                    else
                        return false;
                }
                if (Obj.Collision == CollisionTypes.Rectangular) //If this has circular, but the other one has Rectangular
                {
                    switch (RelativeQuadrant(this.Position, Obj.Position))
                    {
                        case 1:
                            if (Distance(this.Position, ((Engine.PointF)Obj.Position - (Engine.PointF)Obj.CollisionBox / 2)) < this.CollisionRadius)
                                return true;
                            else
                                return false;
                        case 2:
                            if (Distance(this.Position, new PointF((Obj.Position.X + Obj.CollisionBox.X / 2), (Obj.Position.Y - Obj.CollisionBox.Y / 2))) < this.CollisionRadius)
                                return true;
                            else
                                return false;
                        case 3:
                            if (Distance(this.Position, ((Engine.PointF)Obj.Position + (Engine.PointF)Obj.CollisionBox / 2)) < this.CollisionRadius)
                                return true;
                            else
                                return false;
                        case 4:
                            if (Distance(this.Position, new PointF((Obj.Position.X - Obj.CollisionBox.X / 2), (Obj.Position.Y + Obj.CollisionBox.Y / 2))) < this.CollisionRadius)
                                return true;
                            else
                                return false;
                        case 5:
                            if (this.Position.X + CollisionRadius < Obj.Position.X - CollisionBox.X / 2)
                                return false;
                            else
                                return true;
                        case 6:
                            if (this.Position.Y + CollisionRadius < Obj.Position.Y - CollisionBox.Y / 2)
                                return false;
                            else
                                return true;
                        case 7:
                            if (this.Position.X - CollisionRadius > Obj.Position.X + CollisionBox.X / 2)
                                return false;
                            else
                                return true;
                        case 8:
                            if (this.Position.Y - CollisionRadius > Obj.Position.Y - CollisionBox.Y / 2)
                                return false;
                            else
                                return true;
                        case -1:
                            return true;
                        default:
                            return false;
                    }
                }
                return false;
            }
            else //If it goes in here, it must always be Rectangular
            {
                if (Obj.Collision == CollisionTypes.Circular) //If we are rectangular and they are circular, just call the same code as above, except with the positions reversed
                    return Obj.IsCollidedWithGameObject(this);
                else //If we are both rectangular
                    switch (RelativeQuadrant(Position, Obj.Position))
                    {
                        case 1:
                            if (this.Position.X + this.CollisionBox.X / 2 >= Obj.Position.X - Obj.CollisionBox.X / 2)
                            {
                                if (this.Position.Y + this.CollisionBox.Y / 2 >= Obj.Position.Y - Obj.CollisionBox.Y / 2)
                                {
                                    return true;
                                }
                            }
                            return false;
                        case 2:
                            if (this.Position.X - this.CollisionBox.X / 2 <= Obj.Position.X + Obj.CollisionBox.X / 2)
                            {
                                if (this.Position.Y + this.CollisionBox.Y / 2 >= Obj.Position.Y - Obj.CollisionBox.Y / 2)
                                {
                                    return true;
                                }
                            }
                            return false;
                        case 3:
                            if (this.Position.X - this.CollisionBox.X / 2 <= Obj.Position.X + Obj.CollisionBox.X / 2)
                            {
                                if (this.Position.Y - this.CollisionBox.Y / 2 <= Obj.Position.Y + Obj.CollisionBox.Y / 2)
                                {
                                    return true;
                                }
                            }
                            return false;
                        case 4:
                            if (this.Position.X + this.CollisionBox.X / 2 >= Obj.Position.X - Obj.CollisionBox.X / 2)
                            {
                                if (this.Position.Y - this.CollisionBox.Y / 2 <= Obj.Position.Y + Obj.CollisionBox.Y / 2)
                                {
                                    return true;
                                }
                            }
                            return false;
                        case 5:
                            if (this.Position.X + this.CollisionBox.X / 2 >= Obj.Position.X - Obj.CollisionBox.X / 2)
                                return true;
                            return false;
                        case 6:
                            if (this.Position.Y + this.CollisionBox.Y / 2 >= Obj.Position.Y - Obj.CollisionBox.Y / 2)
                                return true;
                            return false;
                        case 7:
                            if (this.Position.X - this.CollisionBox.X / 2 <= Obj.Position.X + Obj.CollisionBox.X / 2)
                                return true;
                            return false;
                        case 8:
                            if (this.Position.Y - this.CollisionBox.Y / 2 <= Obj.Position.Y + Obj.CollisionBox.Y / 2)
                                return true;
                            return false;
                        case -1:
                            return true;
                        default:
                            return false;
                    }

            }
        }
        /*Returns:
        1 if quadrant 1, 2 if quadrant 2, 3 if quadrant 3, 4 if quadrant 4
        5 if it lies on the positive x axis, 6 if it lies on the positive y axis
        7 if it lies on the negative x axis, 8 if it lies on the negative y axis
        -1 if none of the above*/
        public static int RelativeQuadrant(Engine.PointF Pos1, Engine.PointF Pos2)
        {
            if (Pos1.X < Pos2.X)
            {
                if (Pos1.Y < Pos2.Y)
                    return 1;
                if (Pos1.Y > Pos2.Y)
                    return 4;
                else
                    return 5;
            }
            if (Pos1.X > Pos2.X)
            {
                if (Pos1.Y < Pos2.Y)
                    return 2;
                if (Pos1.Y > Pos2.Y)
                    return 3;
                else
                    return 7;
            }
            else
            {
                if (Pos1.Y < Pos2.Y)
                    return 6;
                if (Pos1.Y > Pos2.Y)
                    return 8;
                else
                    return -1;
            }
        }
        private static float Distance(PointF X1, PointF X2)
        {
            return (float)Math.Sqrt((X1.X - X2.X) * (X1.X - X2.X) + (X1.Y - X2.Y) * (X1.Y - X2.Y));
        }
        #endregion
    }
}
