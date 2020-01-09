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

namespace Draeger.Dynamics365.Testautomation.Common.PageObjects.PageElements
{
    public class GridElement : EntityPageBase
    {
        public GridElement(XrmApp xrm, WebClient client = null) : base(xrm, client)
        {
        }

        static class GridElementPointer
        {
            internal static readonly By gridRoot = By.XPath("//div[@wj-part='root']");
            internal static readonly By rows = By.XPath("//div[@header-row-number]");
            internal static readonly By nextPage = By.XPath("//button[contains(@data-id,'moveToNextPage')]");
            internal static readonly By gridCells = By.XPath("//div[@wj-part='cells']");
            internal static readonly By pagingText = By.XPath("//div[@data-id='pagingText']");

            internal static readonly string rowNumberAttr = "header-row-number";
            internal static readonly string gridColCountAttr = "data-col-count";
            internal static readonly string gridRowCountAttr = "data-row-count";
            internal static readonly string gridLabelAttr = "aria-label";
            internal static readonly string innerHTMLAttr = "innerHTML";
            internal static readonly string headerRow = "//div[@role='row' and @aria-label= 'Header']";
            internal static readonly string checkBoxAttr = "aria-label";
            internal static readonly string cellRowColAttr = "data-id";
            internal static readonly string gridInfo = "data-lp-id";
            internal static readonly string cellInfo = "//div[contains(@data-id,'cell-') and @role='gridcell']";

            internal static readonly Regex id = new Regex("(&amp;|&)id=(.+?((?=&)|$))");
            internal static readonly Regex nr = new Regex(@"(\d+)");

        }

        public GridItemInfo SelectGridItem(Guid guid)
        {
            var gridItems = GetGridItems();
            var gridItem = gridItems.FirstOrDefault(g => g.Id == guid);
            while (gridItem == null)
            {
                NextPage();
                gridItems = GetGridItems();
                gridItem = gridItems.FirstOrDefault(g => g.Id == guid);
            }
            var gridRoot = browser.Driver.FindElement(GridElementPointer.gridRoot);
            browser.Driver.ScrollTopReset(gridRoot);
            browser.Driver.ScrollLeftReset(gridRoot);

            var rows = GridElementPointer.rows;
            var itemsPerPage = ItemsPerPage();
            for (int i = 0; i < itemsPerPage;)
            {
                var rowElements = browser.Driver.FindElements(rows);
                var rowMatch = rowElements.FirstOrDefault(r => int.Parse(r.GetAttribute(GridElementPointer.rowNumberAttr)) == gridItem.Index);
                if (rowMatch != null)
                {
                    browser.Driver.ScrollIntoViewIfNeeded(rowMatch);
                    rowMatch.ClickWait();
                    break;
                }
                i += rowElements.Count();
                browser.Driver.ScrollTop(gridRoot);
            }

            return gridItem;
        }
        public void SelectGridItems(int count, params KeyValuePair<string, string>[] attributes)
        {
            while (ItemsSelected() < count)
            {
                var gridItems = GetGridItems();
                var gridRoot = browser.Driver.FindElement(GridElementPointer.gridRoot);
                browser.Driver.ScrollTopReset(gridRoot);
                browser.Driver.ScrollLeftReset(gridRoot);
                var gridItemsToSelect = gridItems.Where(g => g.Attribute.Intersect(attributes).Count() == attributes.Count());
                var rows = GridElementPointer.rows;
                var itemsPerPage = ItemsPerPage();
                for (int i = 0; i < itemsPerPage;)
                {
                    var rowElements = browser.Driver.FindElements(rows);
                    var rowMatch = rowElements.Where(
                        r => gridItemsToSelect.Any(
                            g => g.Index.Equals(int.Parse(r.GetAttribute(GridElementPointer.rowNumberAttr)))));
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
                browser.Driver.FindElement(GridElementPointer.nextPage).ClickWait();
                Thread.Sleep(1000);
            }

            else throw new ElementNotInteractableException("NextPage not available");


        }
        public int ItemsPerPage()
        {
            var gridCells = browser.Driver.FindElement(GridElementPointer.gridCells);
            int gridColCount = int.Parse(gridCells.GetAttribute(GridElementPointer.gridColCountAttr));
            return gridColCount;
        }

        public string GridLabel()
        {
            var gridLabel = browser.Driver.WaitForElement(GridElementPointer.gridCells).GetAttribute(GridElementPointer.gridLabelAttr);
            return gridLabel;
        }

        public Tuple<int, int> ItemsCurrentShown()
        {
            var pagingText = browser.Driver.FindElement(GridElementPointer.pagingText).Text;
            var match = GridElementPointer.nr.Match(pagingText);
            var start = int.Parse(match.Groups[0].Value);
            var end = int.Parse(match.NextMatch().Groups[0].Value);
            return Tuple.Create(start, end);
        }
        public int ItemsCount()
        {
            var pagingText = browser.Driver.FindElement(GridElementPointer.pagingText).Text;
            var match = GridElementPointer.nr.Match(pagingText);
            return int.Parse(match.NextMatch().NextMatch().Groups[0].Value);

        }
        public int ItemsSelected()
        {
            var pagingText = browser.Driver.FindElement(GridElementPointer.pagingText).Text;
            var match = GridElementPointer.nr.Match(pagingText);
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
                    .Where(e=> e.GetAttribute("data-id").EndsWith(col.ToString()) && e.HasAttribute("title"))
                        .Select(cell => cell.GetAttribute("title")));

            return titlesList;
        }


        public List<GridItemInfo> GetGridItems()
        {
            browser.Driver.WaitForLoading();
            XrmApp.ThinkTime(1000);
            var returnList = new List<GridItemInfo>();
            browser.Driver.WaitForElement(By.XPath(AppElements.Xpath[AppReference.Grid.Container]));



            var gridRoot = browser.Driver.FindElement(GridElementPointer.gridRoot);
            var gridCells = browser.Driver.FindElement(GridElementPointer.gridCells);
            int gridColCount = int.Parse(gridCells.GetAttribute(GridElementPointer.gridColCountAttr));
            int gridRowCount = int.Parse(gridCells.GetAttribute(GridElementPointer.gridRowCountAttr));
            List<string> headerList = new List<string>();
            // +1 because the button to select all/one grid item(s) does not count as a column, even if it is one
            for (int i = 0; i < gridColCount+1;)
            {
                XrmApp.ThinkTime(250);
                gridRoot = browser.Driver.FindElement(GridElementPointer.gridRoot);
                var innerHtml = gridRoot.GetAttribute(GridElementPointer.innerHTMLAttr);
                var doc = new HtmlDocument();
                doc.LoadHtml(innerHtml);
                var headerRow = doc.DocumentNode.SelectSingleNode(GridElementPointer.headerRow);
                var headerColumns = headerRow.SelectNodes("./div");
                var headerColumsVisibleCount = headerColumns.Count();

                // retreive header info
                foreach (var hCol in headerColumns)
                {
                    // Is it a checkbox?
                    if (hCol.GetAttributeValue(GridElementPointer.checkBoxAttr, null) != null)
                    {
                        
                        if (headerList.Contains(hCol.GetAttributeValue(GridElementPointer.checkBoxAttr, "")))
                            continue;
                        // true or false
                        headerList.Add(hCol.GetAttributeValue(GridElementPointer.checkBoxAttr, ""));
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
            while (rowCounter < (gridRowCount-1))
            {
                // refresh root
                gridRoot = browser.Driver.FindElement(GridElementPointer.gridRoot);
                var innerHtml = gridRoot.GetAttribute(GridElementPointer.innerHTMLAttr);
                var doc = new HtmlDocument();
                doc.LoadHtml(innerHtml);
                // collection of visible cells
                var cellInfos = doc.DocumentNode.SelectNodes(GridElementPointer.cellInfo);
                

                foreach (var cell in cellInfos)
                {
                    // cell-0-1 = row - col
                    var dataid = cell.GetAttributeValue(GridElementPointer.cellRowColAttr, "");
                    //MscrmControls.Grid.ReadOnlyGrid|entity_control|account|00000000-0000-0000-00aa-000010001001|account|cc-grid|grid-cell-container
                    var datalpid = cell.GetAttributeValue(GridElementPointer.gridInfo, "").Split('|');
                    // title
                    var title = cell.GetAttributeValue("title", "");
                    // div defines a checkbox. can be null
                    var div = cell.SelectSingleNode("./div");

                    // a defines a link. can be null
                    var a = cell.SelectSingleNode("./a");

                    var dataidMatch = GridElementPointer.nr.Match(dataid);
                    // row starts at 0
                    var row = int.Parse(dataidMatch.Groups[0].Value);
                    // col starts at 1
                    var col = int.Parse(dataidMatch.NextMatch().Groups[0].Value);

                    colCounter = col > colCounter ? col : colCounter;
                    rowCounter = row > rowCounter ? row : rowCounter;

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
                        var checkbox = div.GetAttributeValue(GridElementPointer.checkBoxAttr, "");
                        returnList[row].Attribute[headerList[col-1]] = checkbox;
                    }
                    else
                    {
                        returnList[row].Attribute[headerList[col-1]] = title;
                    }

                    if (a != null)
                    {
                        var href = a.GetAttributeValue("href", "");
                        returnList[row].ElementUrl = new Uri(href);
                        if (href.Contains(datalpid[2]))
                        {
                            var idMatch = GridElementPointer.id.Match(href);
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
    public class GridItemInfo
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
    }
}
