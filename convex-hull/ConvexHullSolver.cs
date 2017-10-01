using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace _2_convex_hull
{
    class ConvexHullSolver
    {
        System.Drawing.Graphics g;
        System.Windows.Forms.PictureBox pictureBoxView;
        int pause;

        public ConvexHullSolver(System.Drawing.Graphics g, System.Windows.Forms.PictureBox pictureBoxView, int pause)
        {
            this.g = g;
            this.pictureBoxView = pictureBoxView;
            this.pause = pause;
        }

        public void Refresh()
        {
            // Use this especially for debugging and whenever you want to see what you have drawn so far
            pictureBoxView.Refresh();
        }

        public void Pause(int milliseconds)
        {
            // Use this especially for debugging and to animate your algorithm slowly
            pictureBoxView.Refresh();
            System.Threading.Thread.Sleep(milliseconds);
        }

        private void ShowHull(Hull hull)
        {
            // Create pen.
            Pen pen = new Pen(Color.Red, 1);

            //Draw lines to screen.
            List<PointF> pts = hull.ToList();
            if (pts.Count == 1) return;
            g.DrawLines(pen, pts.ToArray());
        }

        public void Solve(List<System.Drawing.PointF> pointList)
        {
            pointList.Sort(
                delegate (PointF pt1, PointF pt2)       //What does delegate do? I read it's like a pointer to a f(x) but what exactly is happening here?
                {
                    return pt1.X.CompareTo(pt2.X);
                }
            );

            Hull hull = DC(pointList);
            ShowHull(hull);
        }

        private Hull DC(List<PointF> pts)
        {
            if (pts.Count == 1)
                return new Hull(pts[0]);
            
            //divide set of points in half via x-value (they're already sorted by x val)
            int mid = pts.Count / 2;
            List<PointF> ptsL = new List<PointF>();
            List<PointF> ptsR = new List<PointF>();
            for (int i = 0; i < mid; i++)
            {
                ptsL.Add(pts[i]);
            }
            for (int i = mid; i < pts.Count; i++)
            {
                ptsR.Add(pts[i]);
            }

            Hull hullL = DC(ptsL);
            Hull hullR = DC(ptsR);

            return MergeHulls(hullL, hullR);
        }

        private Hull MergeHulls(Hull hullL, Hull hullR)
        {
            calcTangents(hullL, hullR);
            hullL.SetRightmost(hullR.Rightmost());
            //ShowHull(hullL); //Need to remove old hulls so this works
            //Refresh();
            //Pause(pause);
            return hullL;
        }

        //TODO - combine upper and lower tangent functions into one function, using a flag
        //The hull's linkedLists go in a clock-wised fashion
        private void calcTangents(Hull hullL, Hull hullR)
        {
            //Calc upper tangent
            bool checkL = true;
            bool checkR = true;
            Vertex leftUpperTangent = hullL.Rightmost();
            Vertex rightUpperTangent = hullR.Leftmost();
            double currentSlope = slope(leftUpperTangent, rightUpperTangent);
            double newSlope;

            while (checkL || checkR)
            {              
                Vertex Lcurrent = leftUpperTangent;
                while (checkL)
                {
                    Lcurrent = Lcurrent.Previous();
                    //here i may need to check if i've looped through all of the vertices
                    newSlope = slope(Lcurrent, rightUpperTangent); //maybe should be using Rcurrent instead of rightUpperTangent
                    if (newSlope > currentSlope) //if this can be > instead of >=, then i can use Lcurrent.PreviousOrLast above and remove the if(Lcurrent==null) block
                    {
                        leftUpperTangent = Lcurrent;
                        checkR = true;
                        currentSlope = newSlope;
                    }
                    else
                    {
                        checkL = false;
                    }
                }

                Vertex Rcurrent = rightUpperTangent;
                while (checkR)
                {
                    Rcurrent = Rcurrent.Next();
                    //here i may need to check if i've looped through all of the vertices
                    newSlope = slope(Rcurrent, leftUpperTangent); //maybe should be using Lcurrent instead of leftUpperTangent
                    if (newSlope < currentSlope)
                    {
                        rightUpperTangent = Rcurrent;
                        checkL = true;
                        currentSlope = newSlope;
                    }
                    else
                    {
                        checkR = false;
                    }
                }
            }

            calcLowerTangent(hullL, hullR);
            
            leftUpperTangent.SetNext(rightUpperTangent);
            rightUpperTangent.SetPrevious(leftUpperTangent);
        }

        private void calcLowerTangent(Hull hullL, Hull hullR)
        {
            bool checkL = true;
            bool checkR = true;
            Vertex leftLowerTangent = hullL.Rightmost();
            Vertex rightLowerTangent = hullR.Leftmost();
            double currentSlope = slope(leftLowerTangent, rightLowerTangent);
            double newSlope;

            while (checkL || checkR)
            {
                Vertex Lcurrent = leftLowerTangent;
                while (checkL)
                {
                    Lcurrent = Lcurrent.Next();
                    //here i may need to check if i've looped through all of the vertices
                    newSlope = slope(Lcurrent, rightLowerTangent); 
                    if (newSlope < currentSlope) //if this can be < instead of <=, then i can use Lcurrent.PreviousOrLast above and remove the if(Lcurrent==null) block
                    {
                        leftLowerTangent = Lcurrent;
                        checkR = true;
                        currentSlope = newSlope;
                    }
                    else
                    {
                        checkL = false;
                    }
                }

                Vertex Rcurrent = rightLowerTangent;
                while (checkR)
                {
                    Rcurrent = Rcurrent.Previous();
                    //here i may need to check if i've looped through all of the vertices
                    newSlope = slope(Rcurrent, leftLowerTangent);
                    if (newSlope > currentSlope)
                    {
                        rightLowerTangent = Rcurrent;
                        checkL = true;
                        currentSlope = newSlope;
                    }
                    else
                    {
                        checkR = false;
                    }
                }
            }

            leftLowerTangent.SetPrevious(rightLowerTangent);
            rightLowerTangent.SetNext(leftLowerTangent);
        }

        private double slope(Vertex pt1, Vertex pt2)
        {
            return (pt1.Point().Y - pt2.Point().Y) / (pt1.Point().X - pt2.Point().X);
        }
    }


    class Hull
    {
        public Hull(PointF lonePoint)
        {
            Vertex vertex = new Vertex(lonePoint);
            leftmost = rightmost = vertex;
        }

        private Vertex leftmost;
        private Vertex rightmost;

        public Vertex Leftmost() { return leftmost; }
        public Vertex Rightmost() { return rightmost; }

        public void SetLeftmost(Vertex leftmost) { this.leftmost = leftmost; }
        public void SetRightmost(Vertex rightmost) { this.rightmost = rightmost; }

        public List<PointF> ToList()
        {
            List<PointF> pts = new List<PointF>();
            pts.Add(leftmost.Point());
            Vertex current = leftmost;
            while(current.Next() != leftmost)
            {
                current = current.Next();
                pts.Add(current.Point());
            }
            pts.Add(leftmost.Point()); //this is so that you connect the hull at the very end back to the start
            return pts;
        }
    } 

    class Vertex
    {
        public Vertex(PointF lonePoint)
        {
            this.next = this;
            this.previous = this;
            this.point = lonePoint;
        }

        Vertex next;
        Vertex previous;
        PointF point;

        public Vertex Next() { return next; }
        public Vertex Previous() { return previous; }
        public PointF Point() { return point; }
        

        public void SetNext(Vertex next)
        {
            this.next = next;
        }

        public void SetPrevious(Vertex previous)
        {
            this.previous = previous;
        }
    }
}
