using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitServices.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Proto = Revit.Elements;
using DS = Autodesk.DesignScript.Geometry;
using System.Text;
using System.Globalization;


namespace dsDebug
{
    [Transaction(TransactionMode.Manual)]
    public class dsDebugAecom
    {
        internal dsDebugAecom()
        {

        }
        /// <summary>
        /// Sort Views on Sheets by Size
        /// </summary>
        /// <param name="views"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static List<List<View>> SortViewportsBySize([DefaultArgument("{}")] IList<IList> views,
            [DefaultArgument("{}")] int width,
            [DefaultArgument("{}")] int height)
        {
            int iterations = views.Count;
            double feet = 304.8;
            int wi = width;
            int he = height;

            using (AdnRme.ProgressForm form = new AdnRme.ProgressForm("Re-arranging Views", "Processing {0} out of " + iterations.ToString() + " elements", iterations))
            {
                RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentUIDocument.Document);

                List<List<View>> outlines = new List<List<View>>();

                foreach (var el in views)
                {
                    List<View> view_set = new List<View>();

                    if (form.getAbortFlag())
                    {
                        return null;
                    }

                    foreach (var e in el)
                    {
                        View v = e as ViewSection;

                        double w = (v.Outline.Max.U - v.Outline.Min.U) * feet;
                        double h = (v.Outline.Max.V - v.Outline.Min.V) * feet;

                        Parameter view_comment = v.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION);

                        if ((w < wi / 2) && (h < he / 2))
                        {
                            view_comment.Set("A");
                        }
                        else if ((w > wi / 2) && (w < wi) && (he < h / 4))
                        {
                            view_comment.Set("B");
                        }
                        else if ((w > wi / 2) && (w < wi) && (he < h / 2))
                        {
                            view_comment.Set("C");
                        }
                        else if ((w < wi) && (h < he))
                        {
                            view_comment.Set("D");
                        }
                        else
                        {
                            view_comment.Set("E");
                        }

                        view_set.Add(v);
                    }

                    form.Increment();

                    outlines.Add(view_set);

                    RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();
                }

                return outlines;
            }
        }
    }
}

