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

            /*
            ShowHull(hullL);
            ShowHull(hullR);
            */

            return MergeHulls(hullL, hullR);
        }

        private Hull MergeHulls(Hull hullL, Hull hullR)
        {
            return null;





            bool checkL = true;
            bool checkR = true;
            //getLeftUpperTangent();
            while(checkL)
            {
                //if slope of next isn't steeper
                checkL = false;


            }
            while(checkR)
            {

            }
            return null;
        }

        private void ShowHull(Hull hull)
        {
            // Create pen.
            Pen pen = new Pen(Color.Red, 3);

            //Draw lines to screen.
            g.DrawLines(pen, hull.ToArray());
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

    class Hull
    {
        public Hull(IEnumerable<PointF> pts)
        {
            hull = new LinkedList<PointF>(pts);
            /*leftmost = new Vertex(pts[0]);

            for(int i=1; i<pts.Count; i++)
            {
                pts[i-1].SetNext(new Vertex(pts[i]));
            } */
        }

        private LinkedList<PointF> hull;

        public PointF[] ToArray()
        {
            PointF[] array = new PointF[hull.Count];
            hull.CopyTo(array, 0);
            return array;
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
