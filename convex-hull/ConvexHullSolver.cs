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

        public ConvexHullSolver(System.Drawing.Graphics g, System.Windows.Forms.PictureBox pictureBoxView)
        {
            this.g = g;
            this.pictureBoxView = pictureBoxView;
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
            Pen pen = new Pen(Color.Red, 3);

            //Draw lines to screen.
            PointF[] pts = new PointF[hull.Count];
            hull.CopyTo(pts, 0);
            g.DrawLines(pen, pts);
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



            
        }

        private Hull DC(List<PointF> pts)
        {
            if (pts.Count == 1)
                return new Hull(pts);
            
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
            Hull combination = calcUpperTangent(hullL, hullR);
            //calcLowerTangent(combination); ???
            return null;
        }

        private Hull calcUpperTangent(Hull hullL, Hull hullR)
        {
            bool checkL = true;
            bool checkR = true;
            LinkedListNode<PointF> leftUpperTangent = hullL.Rightmost();
            LinkedListNode<PointF> rightUpperTangent = hullR.Leftmost();
            double currentSlope = slope(leftUpperTangent, rightUpperTangent);
            double newSlope;

            while (checkL || checkR)
            {              
                LinkedListNode<PointF> Lcurrent = leftUpperTangent;
                while (checkL)
                {
                    Lcurrent = Lcurrent.Previous; // OrLast(); -> don't think this is necessary here
                    if(Lcurrent == null) //you've reached the end of the linked list
                    {
                        checkL = false;
                        break;
                    }
                    newSlope = slope(Lcurrent, rightUpperTangent); //maybe should be using Rcurrent instead of rightUpperTangent
                    if (newSlope >= currentSlope) //if this can be > instead of >=, then i can use Lcurrent.PreviousOrLast above and remove the if(Lcurrent==null) block
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

                LinkedListNode<PointF> Rcurrent = rightUpperTangent;
                while (checkR)
                {
                    Rcurrent = Rcurrent.Next; // OrFirst(); -> don't think this is necessary here
                    if (Rcurrent == null) //you've reached the end of the linked list
                    {
                        checkR = false;
                        break;
                    }
                    newSlope = slope(Rcurrent, leftUpperTangent); //maybe should be using Lcurrent instead of leftUpperTangent
                    if (newSlope <= currentSlope)
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
            
            //TODO still need to link left upper tangent with right upper tangent

            return hullL;
        }

        private double slope(LinkedListNode<PointF> pt1, LinkedListNode<PointF> pt2)
        {
            return (pt1.Value.Y - pt2.Value.Y) / (pt1.Value.X - pt2.Value.X);
        }
    }


     class Hull : LinkedList<PointF>
    {
        private LinkedListNode<PointF> leftmost;
        private LinkedListNode<PointF> rightmost;

        public Hull(List<PointF> pts) : base(pts) //this will only ever be one point
        {
            leftmost = rightmost = new LinkedListNode<PointF>(pts[0]);
        }

        public Hull(Hull template) : base(template)
        {
            leftmost = template.leftmost;
        }

        public LinkedListNode<PointF> Leftmost()
        {
            return leftmost;
        }
        public LinkedListNode<PointF> Rightmost()
        {
            return rightmost;
        }

        public void SetRightmost(LinkedListNode<PointF> rightmost)
        {
            this.rightmost = rightmost;
        }

        public PointF[] ToArray()
        {
            PointF[] array = new PointF[this.Count];
            this.CopyTo(array, 0);
            return array;
        }
    } 


    /** This class represents extended methods added to the  LinkedListNode class. 
     *  These methods make the builtin doubly linked list essentially circular
     *  See https://stackoverflow.com/questions/716256/creating-a-circularly-linked-list-in-c
     *  and https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods
     */
    static class CircularLinkedList
    {
        public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> current)
        {
            return current.Next ?? current.List.First;
        }

        public static LinkedListNode<T> PreviousOrLast<T>(this LinkedListNode<T> current)
        {
            return current.Previous ?? current.List.Last;
        }
    }

    /*class Vertex
    {
        public Vertex(PointF pt)
        {
            this.pt = pt;
        }

        Vertex next;
        Vertex prev;
        PointF pt;

        public void SetNext(Vertex next)
        {
            this.next = next;
        }
    } */
}
