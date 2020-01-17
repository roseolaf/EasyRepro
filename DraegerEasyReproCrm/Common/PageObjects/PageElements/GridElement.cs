using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Draeger.Dynamics365.Testautomation.ExtensionMethods;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using HtmlAgilityPack;
using System.Threading;
using Microsoft.TeamFoundation.Common;
using Grid = System.Windows.Controls.Grid;

namespace Draeger.Dynamics365.Testautomation.Common.PageObjects.PageElements
{
    public class GridElement : EntityPageBase
    {
        public GridElement(XrmApp xrm, WebClient client = null, string gridName = "") : base(xrm, client)
        {
            this.gridName = gridName;
        }
        internal string gridName { get; set; }
        static class GridElementLocators
        {
            internal static readonly By gridRoot = By.XPath(".//div[@wj-part='root']");
            internal static readonly By rows = By.XPath(".//div[@header-row-number]");
            internal static readonly By nextPage = By.XPath(".//button[contains(@data-id,'moveToNextPage')]");
            internal static readonly By gridCells = By.XPath(".//div[@wj-part='cells']");
            internal static readonly By pagingText = By.XPath(".//div[@data-id='pagingText']");
            internal static readonly By button = By.XPath(".//button");

            internal static readonly string gridName = "//div[contains(@id,\"dataSetRoot\") and contains(@id,\"NAME_outer\")]";
            internal static readonly string rowNumberAttr = "header-row-number";
            internal static readonly string gridColCountAttr = "data-col-count";
            internal static readonly string gridRowCountAttr = "data-row-count";
            internal static readonly string gridLabelAttr = "aria-label";
            internal static readonly string innerHTMLAttr = "innerHTML";
            internal static readonly string headerRow = ".//div[@role='row' and @aria-label= 'Header']";
            internal static readonly string checkBoxAttr = "aria-label";
            internal static readonly string cellRowColAttr = "data-id";
            internal static readonly string gridInfo = "data-lp-id";
            internal static readonly string cellInfo = ".//div[contains(@data-id,'cell-') and @role='gridcell']";

            internal static readonly Regex id = new Regex("(&amp;|&)id=(.+?((?=&)|$))");
            internal static readonly Regex nr = new Regex(@"(\d+)");

        }

        public GridItemInfo SelectGridItem(Guid guid)
        {

            var gridNameLocator = GridElementLocators.gridName.Replace("NAME", gridName);
            var gridBaseElement = browser.Driver.WaitForElement(By.XPath(gridNameLocator));

            var gridItems = GetGridItems();
            var gridItem = gridItems.FirstOrDefault(g => g.Id == guid);
            while (gridItem == null)
            {
                NextPage();
                gridItems = GetGridItems();
                gridItem = gridItems.FirstOrDefault(g => g.Id == guid);
            }
            var gridRoot = gridBaseElement.FindElement(GridElementLocators.gridRoot);
            browser.Driver.ScrollTopReset(gridRoot);
            browser.Driver.ScrollLeftReset(gridRoot);

            var rows = GridElementLocators.rows;
            var itemsPerPage = ItemsPerPage();
            for (int i = 0; i < itemsPerPage;)
            {
                var rowElements = gridBaseElement.FindElements(rows);
                var rowMatch = rowElements.FirstOrDefault(r => int.Parse(r.GetAttribute(GridElementLocators.rowNumberAttr)) == gridItem.Index);
                if (rowMatch != null)
                {
                    browser.Driver.ScrollIntoViewIfNeeded(rowMatch);
                    browser.ThinkTime(500);
                    rowMatch.ClickWait();
                    break;
                }
                i += rowElements.Count();
                browser.Driver.ScrollTop(gridRoot);
            }
            browser.ThinkTime(500);
            return gridItem;
        }

        public GridItemInfo SelectGridItem(GridItemInfo git)
        {

            var gridNameLocator = GridElementLocators.gridName.Replace("NAME", gridName);
            var gridBaseElement = browser.Driver.WaitForElement(By.XPath(gridNameLocator));

            var gridItems = GetGridItems();
            var gridItem = gridItems.FirstOrDefault(g => g.Equals(git));

            while (gridItem == null)
            {
                NextPage();
                gridItems = GetGridItems();
                gridItem = gridItems.FirstOrDefault(g => g == git);
            }
            var gridRoot = gridBaseElement.FindElement(GridElementLocators.gridRoot);
            browser.Driver.ScrollTopReset(gridRoot);
            browser.Driver.ScrollLeftReset(gridRoot);

            var rows = GridElementLocators.rows;
            var itemsPerPage = ItemsPerPage();
            for (int i = 0; i < itemsPerPage;)
            {
                var rowElements = gridBaseElement.FindElements(rows);
                var rowMatch = rowElements.FirstOrDefault(r => int.Parse(r.GetAttribute(GridElementLocators.rowNumberAttr)) == gridItem.Index);
                if (rowMatch != null)
                {
                    browser.Driver.ScrollIntoViewIfNeeded(rowMatch);
                    browser.ThinkTime(500);
                    rowMatch.ClickWait();
                    break;
                }
                i += rowElements.Count();
                browser.Driver.ScrollTop(gridRoot);
            }
            browser.ThinkTime(500);
            return gridItem;
        }

        public List<GridItemInfo> SelectGridItems(List<GridItemInfo> gridItemList)
        {

            var gridNameLocator = GridElementLocators.gridName.Replace("NAME", gridName);
            var gridBaseElement = browser.Driver.WaitForElement(By.XPath(gridNameLocator));

            var gridItems = GetGridItems();
            var selectGridItems = gridItems.Where(gridItemList.Contains).ToList();

    
            var gridRoot = gridBaseElement.FindElement(GridElementLocators.gridRoot);
            browser.Driver.ScrollTopReset(gridRoot);
            browser.Driver.ScrollLeftReset(gridRoot);

            var rows = GridElementLocators.rows;

            foreach (var gI in selectGridItems)
            {
                ScrollToGridItem(gI);
                var rowElements = gridBaseElement.FindElements(rows);
                var rowMatch = rowElements.FirstOrDefault(r => int.Parse(r.GetAttribute(GridElementLocators.rowNumberAttr)) == gI.Index);
                if (rowMatch != null)
                {
                    browser.Driver.ScrollIntoViewIfNeeded(rowMatch);
                    browser.ThinkTime(500);
                    rowMatch.ClickWait();
                }
            }

            browser.ThinkTime(500);
            return selectGridItems;
        }

        public List<GridItemInfo> SelectGridItems(List<int> gridItemIndexList)
        {

            var gridNameLocator = GridElementLocators.gridName.Replace("NAME", gridName);
            var gridBaseElement = browser.Driver.WaitForElement(By.XPath(gridNameLocator));

            var gridItems = GetGridItems();
            var returnList = gridItemIndexList.Select(i => gridItems[i]).ToList();

            var gridRoot = gridBaseElement.FindElement(GridElementLocators.gridRoot);
            browser.Driver.ScrollTopReset(gridRoot);
            browser.Driver.ScrollLeftReset(gridRoot);

            var rows = GridElementLocators.rows;
            foreach (var gI in returnList)
            {
                ScrollToGridItem(gI);
                var rowElements = gridBaseElement.FindElements(rows);
                var rowMatch = rowElements.FirstOrDefault(r => int.Parse(r.GetAttribute(GridElementLocators.rowNumberAttr)) == gI.Index);
                if (rowMatch != null)
                {
                    browser.Driver.ScrollIntoViewIfNeeded(rowMatch);
                    browser.ThinkTime(500);
                    rowMatch.ClickWait();
                }
            }

            browser.ThinkTime(500);
            return returnList;
        }

        public void ScrollToGridItem(GridItemInfo gridItem)
        {
            var gridNameLocator = GridElementLocators.gridName.Replace("NAME", gridName);
            var gridBaseElement = browser.Driver.WaitForElement(By.XPath(gridNameLocator));
            var gridRoot = gridBaseElement.FindElement(GridElementLocators.gridRoot);

            var rowElements = gridBaseElement.FindElements(GridElementLocators.rows);
            var rowMatch = rowElements.Select(r => int.Parse(r.GetAttribute(GridElementLocators.rowNumberAttr))).ToList();

            var gridItemScrollPosition = (int)Math.Floor((double)(gridItem.Index / rowMatch.Count));
            browser.Driver.ScrollTopIndex(gridRoot,gridItemScrollPosition);

        }

     

        public void SelectGridItem(params KeyValuePair<string, string>[] attributes)
        {
            var gridNameLocator = GridElementLocators.gridName.Replace("NAME", gridName);
            var gridBaseElement = browser.Driver.WaitForElement(By.XPath(gridNameLocator));

            var gridItems = GetGridItems();
            var gridRoot = gridBaseElement.FindElement(GridElementLocators.gridRoot);
            var attributesDict = attributes.ToDictionary(x => x.Key, x => x.Value);
            browser.Driver.ScrollTopReset(gridRoot);
            browser.Driver.ScrollLeftReset(gridRoot);
            var gridItemsToSelect = gridItems.First(g =>
            {
                var intersectCount = g.Attribute.Intersect(attributesDict).Count();
                return intersectCount == attributesDict.Count();
            });
            SelectGridItem(gridItemsToSelect.Id);

        }
        public void SelectGridItems(int count, params KeyValuePair<string, string>[] attributes)
        {
            var gridNameLocator = GridElementLocators.gridName.Replace("NAME", gridName);
            var gridBaseElement = browser.Driver.WaitForElement(By.XPath(gridNameLocator));

            while (ItemsSelected() < count)
            {
                var gridItems = GetGridItems();
                var gridRoot = gridBaseElement.FindElement(GridElementLocators.gridRoot);
                browser.Driver.ScrollTopReset(gridRoot);
                browser.Driver.ScrollLeftReset(gridRoot);
                var gridItemsToSelect = gridItems.Where(g => g.Attribute.Intersect(attributes).Count() == attributes.Count());
                var rows = GridElementLocators.rows;
                var itemsPerPage = ItemsPerPage();
                for (int i = 0; i < itemsPerPage;)
                {
                    var rowElements = gridBaseElement.FindElements(rows);
                    var rowMatch = rowElements.Where(
                        r => gridItemsToSelect.Any(
                            g => g.Index.Equals(int.Parse(r.GetAttribute(GridElementLocators.rowNumberAttr)))));
                    foreach (var rM in rowMatch)
                    {
                        browser.Driver.ScrollIntoViewIfNeeded(rM);
                        rM.ClickWait();
                        if (ItemsSelected() == count)
                            break;
                    }
                    if (ItemsSelected() == count)
                        break;

                    i += rowElements.Count();
                    browser.Driver.ScrollTop(gridRoot);
                }

                if (ItemsSelected() < count)
                    NextPage();
            }
        }

        public void NextPage()
        {
            if (ItemsCurrentShown().Item2 != ItemsCount())
            {

                var gridNameLocator = GridElementLocators.gridName.Replace("NAME", gridName);
                var gridBaseElement = browser.Driver.WaitForElement(By.XPath(gridNameLocator));

                gridBaseElement.FindElement(GridElementLocators.nextPage).ClickWait();
                Thread.Sleep(1000);
            }

            else throw new ElementNotInteractableException("NextPage not available");


        }
        public int ItemsPerPage()
        {
            var gridNameLocator = GridElementLocators.gridName.Replace("NAME", gridName);
            var gridBaseElement = browser.Driver.WaitForElement(By.XPath(gridNameLocator));

            var gridCells = gridBaseElement.FindElement(GridElementLocators.gridCells);
            int gridRowCount = int.Parse(gridCells.GetAttribute(GridElementLocators.gridRowCountAttr));
            return gridRowCount;
        }

        public string GridLabel()
        {

            var gridNameLocator = GridElementLocators.gridName.Replace("NAME", gridName);
            var gridBaseElement = browser.Driver.WaitForElement(By.XPath(gridNameLocator));

            var gridLabel = gridBaseElement.FindElement(GridElementLocators.gridCells).GetAttribute(GridElementLocators.gridLabelAttr);
            return gridLabel;
        }

        public Tuple<int, int> ItemsCurrentShown()
        {
            var gridNameLocator = GridElementLocators.gridName.Replace("NAME", gridName);
            var gridBaseElement = browser.Driver.WaitForElement(By.XPath(gridNameLocator));

            var pagingText = gridBaseElement.FindElement(GridElementLocators.pagingText).Text;
            var match = GridElementLocators.nr.Match(pagingText);
            var start = int.Parse(match.Groups[0].Value);
            var end = int.Parse(match.NextMatch().Groups[0].Value);
            return Tuple.Create(start, end);
        }
        public int ItemsCount()
        {
            var gridNameLocator = GridElementLocators.gridName.Replace("NAME", gridName);
            var gridBaseElement = browser.Driver.WaitForElement(By.XPath(gridNameLocator));

            var pagingText = gridBaseElement.FindElement(GridElementLocators.pagingText).Text;
            var match = GridElementLocators.nr.Match(pagingText);
            return int.Parse(match.NextMatch().NextMatch().Groups[0].Value);

        }
        public int ItemsSelected()
        {
            var gridNameLocator = GridElementLocators.gridName.Replace("NAME", gridName);
            var gridBaseElement = browser.Driver.WaitForElement(By.XPath(gridNameLocator));

            var pagingText = gridBaseElement.FindElement(GridElementLocators.pagingText).Text;
            var match = GridElementLocators.nr.Match(pagingText);
            return int.Parse(match.NextMatch().NextMatch().NextMatch().Groups[0].Value);
        }


        public List<string> GetCellTitles(string gridId, int col)
        {
            // Wait 1.5 seconds to overcome staleness 
            XrmApp.ThinkTime(1500);
            var gridElement = browser.Driver.WaitForElement(By.Id(gridId));

            var titlesList = new List<string>();
            titlesList.AddRange(gridElement
                .FindElements(By.XPath(".//div[contains(@data-id,'cell-') and @role='gridcell']"))
                    .Where(e => e.GetAttribute("data-id").EndsWith(col.ToString()) && e.HasAttribute("title"))
                        .Select(cell => cell.GetAttribute("title")));

            return titlesList;
        }

        public void Button(string name, string subName = "")
        {
            var gridNameLocator = GridElementLocators.gridName.Replace("NAME", gridName);
            var gridBaseElement = browser.Driver.WaitForElement(By.XPath(gridNameLocator));

            var buttons = gridBaseElement.FindElements(GridElementLocators.button);

            buttons.First(element => element.Text.ToLower().Contains(name.ToLower())).ClickWait();
            if (!subName.IsNullOrEmpty())
                buttons.First(element => element.Text.ToLower().Contains(subName.ToLower())).ClickWait();
        }

        public List<GridItemInfo> GetGridItems()

        {
            browser.Driver.WaitForLoading();
            XrmApp.ThinkTime(1000);
            var returnList = new List<GridItemInfo>();
            browser.Driver.WaitForElement(By.XPath(AppElements.Xpath[AppReference.Grid.Container]));


            var gridNameLocator = GridElementLocators.gridName.Replace("NAME", gridName);
            var gridBaseElement = browser.Driver.WaitForElement(By.XPath(gridNameLocator));


            var gridRoot = gridBaseElement.FindElement(GridElementLocators.gridRoot);
            var gridCells = gridBaseElement.FindElement(GridElementLocators.gridCells);
            int gridColCount = int.Parse(gridCells.GetAttribute(GridElementLocators.gridColCountAttr));
            int gridRowCount = int.Parse(gridCells.GetAttribute(GridElementLocators.gridRowCountAttr));
            List<string> headerList = new List<string>();
            // +1 because the button to select all/one grid item(s) does not count as a column, even if it is one
            for (int i = 0; i < gridColCount + 1;)
            {
                XrmApp.ThinkTime(250);
                gridRoot = gridBaseElement.FindElement(GridElementLocators.gridRoot);
                var innerHtml = gridRoot.GetAttribute(GridElementLocators.innerHTMLAttr);
                var doc = new HtmlDocument();
                doc.LoadHtml(innerHtml);
                var headerRow = doc.DocumentNode.SelectSingleNode(GridElementLocators.headerRow);
                var headerColumns = headerRow.SelectNodes("./div");
                var headerColumsVisibleCount = headerColumns.Count();

                // retreive header info
                foreach (var hCol in headerColumns)
                {
                    // Is it a checkbox?
                    if (hCol.GetAttributeValue(GridElementLocators.checkBoxAttr, null) != null)
                    {

                        if (headerList.Contains(hCol.GetAttributeValue(GridElementLocators.checkBoxAttr, "")))
                            continue;
                        // true or false
                        headerList.Add(hCol.GetAttributeValue(GridElementLocators.checkBoxAttr, ""));
                    }
                    else
                    {
                        if (headerList.Contains(hCol.InnerText))
                            continue;
                        headerList.Add(hCol.InnerText);
                    }

                }

                browser.Driver.ScrollLeft(gridRoot);
                i += headerColumsVisibleCount;

            }
            browser.Driver.ScrollLeftReset(gridRoot);

            // used for scrolling
            var colCounter = 0;
            var rowCounter = 0;

            // zero base counter
            while (rowCounter < (gridRowCount))
            {
                // refresh root
                gridRoot = gridBaseElement.FindElement(GridElementLocators.gridRoot);
                var innerHtml = gridRoot.GetAttribute(GridElementLocators.innerHTMLAttr);
                var doc = new HtmlDocument();
                doc.LoadHtml(innerHtml);
                // collection of visible cells
                var cellInfos = doc.DocumentNode.SelectNodes(GridElementLocators.cellInfo);


                foreach (var cell in cellInfos)
                {
                    // cell-0-1 = row - col
                    var dataid = cell.GetAttributeValue(GridElementLocators.cellRowColAttr, "");
                    //MscrmControls.Grid.ReadOnlyGrid|entity_control|account|00000000-0000-0000-00aa-000010001001|account|cc-grid|grid-cell-container
                    var datalpid = cell.GetAttributeValue(GridElementLocators.gridInfo, "").Split('|');
                    // title
                    var title = cell.GetAttributeValue("title", "");
                    // div defines a checkbox. can be null
                    var div = cell.SelectSingleNode("./div");

                    // a defines a link. can be null
                    var a = cell.SelectSingleNode("./a");

                    var dataidMatch = GridElementLocators.nr.Match(dataid);
                    // row starts at 0
                    var row = int.Parse(dataidMatch.Groups[0].Value);
                    // col starts at 1
                    var col = int.Parse(dataidMatch.NextMatch().Groups[0].Value);

                    colCounter = col > colCounter ? col : colCounter;
                    rowCounter = row + 1 > rowCounter ? row + 1 : rowCounter;

                    // items are added chaotically to the GridItemInfo, so init the row
                    if (returnList.ElementAtOrDefault(row) == null)
                        returnList.Insert(row, new GridItemInfo());


                    if (col == 1)
                    {
                        var gridLink =
                         $"{new Uri(browser.Driver.Url).Scheme}://{new Uri(browser.Driver.Url).Authority}/main.aspx?etn={datalpid[2]}&pagetype=entityrecord&id=%7B{datalpid[3]}%7D";

                        returnList[row].EntityName = datalpid[2];
                        returnList[row].GridUrl = new Uri(gridLink);
                        returnList[row].Index = row;

                    }


                    if (div != null)
                    {
                        var checkbox = div.GetAttributeValue(GridElementLocators.checkBoxAttr, "");
                        returnList[row].Attribute[headerList[col - 1]] = checkbox;
                    }
                    else
                    {
                        returnList[row].Attribute[headerList[col - 1]] = title;
                    }

                    if (a != null)
                    {
                        var href = a.GetAttributeValue("href", "");
                        returnList[row].ElementUrl = new Uri(href);
                        if (href.Contains(datalpid[2]))
                        {
                            var idMatch = GridElementLocators.id.Match(href);
                            returnList[row].Id = Guid.Parse(idMatch.Groups[2].Value);
                        }
                    }

                }
                // scroll right until we have seen all columns
                if (colCounter < gridColCount)
                    browser.Driver.ScrollLeft(gridRoot);
                else
                {

                    browser.Driver.ScrollLeftReset(gridRoot);
                    colCounter = 0;
                    // scroll down to see more rows
                    browser.Driver.ScrollTop(gridRoot);
                }

                XrmApp.ThinkTime(250);

            }

            browser.Driver.ScrollLeftReset(gridRoot);
            browser.Driver.ScrollTopReset(gridRoot);
            return returnList;
        }
    }
    public class GridItemInfo : IEquatable<GridItemInfo>
    {
        public GridItemInfo()
        {
            Attribute = new Dictionary<string, string>();
        }
        public Guid Id { get; set; }
        public string EntityName { get; set; }
        public int Index { get; set; }
        public Uri GridUrl { get; set; }
        public Uri ElementUrl { get; set; }
        public Dictionary<string, string> Attribute { get; set; }
        public bool Equals(GridItemInfo gii)
        {
            return this.Id == gii.Id
                   && this.Attribute.Count == gii.Attribute.Count
                   && this.Attribute.All(attr => gii.Attribute.ContainsValue(attr.Value))
                   && this.ElementUrl == gii.ElementUrl
                   && this.EntityName == gii.EntityName
                   && this.GridUrl == gii.GridUrl;
        }

        public override bool Equals(object o)
        {
            var gii = o as GridItemInfo;
            return this.Id == gii.Id
                   && this.Attribute.Count == gii.Attribute.Count
                   && this.Attribute.All(attr => gii.Attribute.ContainsValue(attr.Value))
                   && this.ElementUrl == gii.ElementUrl
                   && this.EntityName == gii.EntityName
                   && this.GridUrl == gii.GridUrl;
        }
 

        public override int GetHashCode()
        {
            return this != null ? this.Attribute.GetHashCode() : 0;
        }


        public static bool operator == (GridItemInfo gridItem1, GridItemInfo gridItem2)
        {
            if ((object)gridItem1 == null && (object)gridItem2 == null)
                return true;
            if ((object)gridItem1 == null | (object)gridItem2 == null)
                return false;
            return gridItem1.Equals(gridItem2);
        }
        public static bool operator != (GridItemInfo gridItem1, GridItemInfo gridItem2)
        {
            if ((object)gridItem1 == null && (object)gridItem2 == null)
                return false;
            if ((object)gridItem1 == null | (object)gridItem2 == null)
                return true;
            return !gridItem1.Equals(gridItem2);
        }
    }
    //public class GridItemEqualityComparer : IEqualityComparer<GridItemInfo>
    //{
    //    public bool Equals(GridItemInfo x, GridItemInfo y)
    //    {
    //        return x.Id == y.Id
    //               && x.Attribute.Count == y.Attribute.Count
    //               && x.Attribute.All(attr => y.Attribute.ContainsValue(attr.Value))
    //               && x.ElementUrl == y.ElementUrl
    //               && x.EntityName == y.EntityName
    //               && x.GridUrl == y.GridUrl;
    //    }

    //    public int GetHashCode(GridItemInfo obj)
    //    {

    //        return obj != null ? obj.Attribute.GetHashCode() : 0;
    //    }
    //}
}
