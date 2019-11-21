// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace Microsoft.Dynamics365.UIAutomation.Api.Pages
{
    /// <summary>
    /// Activity feed page.
    /// </summary>
    public class ActivityFeed : XrmPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityFeed"/> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        public ActivityFeed(InteractiveBrowser browser)
            : base(browser)
        {
            this.SwitchToContent();
        }

        public enum Tab
        {
            Posts,
            Activities,
            Notes
        }
        public enum Status
        {
            All,
            InProgress,
            Overdue
        }

        public enum Activities
        {
            Appointment,
            Email
        }


        /// <summary>
        /// Selects the tab.
        /// </summary>
        /// <param name="tabname">The TabName you want to select.</param>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        /// <example>xrmBrowser.ActivityFeed.SelectTab(Api.Pages.XrmActivityFeedPage.Tab.Activities);</example>
        public BrowserCommandResult<bool> SelectTab(Tab tabname, int thinkTime = Constants.DefaultThinkTime)
        {
            Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions($"Select Tab on the Record Wall"), driver =>
            {
                if (!driver.ElementExists(By.XPath(Elements.Xpath[Reference.ActivityFeed.NotesControl])))
                    throw new InvalidOperationException("The Record Wall is not available.");

                var notesControl = driver.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.NotesControl]));
                var tabs = notesControl.FindElements(By.TagName("a"));

                var tab = tabs.FirstOrDefault(x => x.Text == tabname.ToString().ToUpper());

                if (tab == null)
                    throw new InvalidOperationException("The Tab is not available.");

                tab.ClickWait();

                return true;
            });
        }

        /// <summary>
        /// Adds Notes to the activity feed
        /// </summary>
        /// <param name="noteText">The NoteText you want to add</param>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        /// <example>xrmBrowser.ActivityFeed.AddNote("Test Add Note");</example>
        public BrowserCommandResult<bool> AddNote(string noteText, int thinkTime = Constants.DefaultThinkTime)
        {
            Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions($"Add Note"), driver =>
            {
                if (!driver.ElementExists(By.XPath(Elements.Xpath[Reference.ActivityFeed.NotesWall])))
                    throw new InvalidOperationException("The Notes Wall is not available. Please check that the Notes tab is selected.");

                var wall = driver.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.NotesWall]));
                var text = wall.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.NotesText]));
                text.ClickWait();

                var textArea = wall.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.NotesText]));
                textArea.ClickWait();
                textArea.SendKeysWait(noteText);

                var post = driver.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.NotesDone]));
                var done = post.FindElement(By.Id("postButton"));
                done.ClickWait();

                return true;
            });
        }

        /// <summary>
        /// Adds Post to the activity feed
        /// </summary>
        /// <param name="postText">The Text you want to post</param>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        /// <example>xrmBrowser.ActivityFeed.AddPost("Test Add Post");</example>
        public BrowserCommandResult<bool> AddPost(string postText, int thinkTime = Constants.DefaultThinkTime)
        {
            Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions($"Add Post"), driver =>
            {
                if (!driver.ElementExists(By.XPath(Elements.Xpath[Reference.ActivityFeed.PostWall])))
                    throw new InvalidOperationException("The Post Wall is not available. Please check that the Posts tab is selected.");

                var wall = driver.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.PostWall]));
                var text = wall.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.PostText]));

                text.ClickWait();
                text.SendKeysWait(postText);

                wall.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.PostButton])).ClickWait();

                return true;
            });
        }

        /// <summary>
        /// Filters Activities by Status
        /// </summary>
        /// <param name="status">The Status</param>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        /// <example>xrmBrowser.ActivityFeed.FilterActivitiesByStatus(Api.Pages.XrmActivityFeedPage.Status.Overdue);</example>
        public BrowserCommandResult<bool> FilterActivitiesByStatus(Status status, int thinkTime = Constants.DefaultThinkTime)
        {
            Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions($"Filter Activity by Status"), driver =>
            {
                if (!driver.ElementExists(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityWall])))
                    throw new InvalidOperationException("The Activity Wall is not available. Please check that the Activities tab is selected.");

                var wall = driver.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityWall]));
                wall.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityStatusFilter])).ClickWait();

                var dialog = driver.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityStatusFilterDialog]));
                var statusList = dialog.FindElements(By.TagName("li"));
                statusList.Where(x => x.Text.ToLower().Replace(" ", "") == status.ToString().ToLower()).FirstOrDefault()?.ClickWait();

                return true;
            });
        }

        /// <summary>
        /// Opens Activities Associated View
        /// </summary>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        public BrowserCommandResult<bool> OpenActivitiesAssociatedView(int thinkTime = Constants.DefaultThinkTime)
        {
            Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions($"Open Activities Associated View"), driver =>
            {
                if (!driver.ElementExists(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityWall])))
                    throw new InvalidOperationException("The Activity Wall is not available. Please check that the Activities tab is selected.");

                driver.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityAssociatedView])).ClickWait();

                return true;
            });
        }

        /// <summary>
        /// Adds PhoneCall to the activity feed
        /// </summary>
        /// <param name="description">The description</param>
        /// <param name="leftVoiceMail">The leftVoiceMail</param>
        /// <param name="outgoing">The outgoing</param>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        /// <example>xrmBrowser.ActivityFeed.AddPhoneCall("Test Phone call Description",false);</example>
        public BrowserCommandResult<bool> AddPhoneCall(string description, bool leftVoiceMail, bool outgoing = true, int thinkTime = Constants.DefaultThinkTime)
        {
            Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions($"Add Phone Call from Activity Feed"), driver =>
            {
                if (!driver.ElementExists(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityWall])))
                    throw new InvalidOperationException("The Activity Feed is not available. Please check that the Activities tab is selected.");

                driver.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityAddPhoneCall])).ClickWait();

                this.SetValue(Elements.ElementId[Reference.ActivityFeed.ActivityPhoneCallDescriptionId], description);
                if (leftVoiceMail)
                {
                    driver.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.VoiceMail])).ClickWait();
                }

                if (!outgoing)
                    driver.FindElement(By.Id(Elements.ElementId[Reference.ActivityFeed.ActivityPhoneCallDirectionId])).ClickWait();;

                driver.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityPhoneCallOk])).ClickWait();;

                return true;
            });
        }

        /// <summary>
        /// Adds task to the activity feed
        /// </summary>
        /// <param name="subject">The subject</param>
        /// <param name="description">The description</param>
        /// <param name="dueDate">The dueDate</param>
        /// <param name="priority">The priority</param>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        public BrowserCommandResult<bool> AddTask(string subject, string description, DateTime dueDate, OptionSet priority, int thinkTime = Constants.DefaultThinkTime)
        {
            Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions($"Add Task from Activity Feed"), driver =>
            {
                if (!driver.ElementExists(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityWall])))
                    throw new InvalidOperationException("The Activity Feed is not available. Please check that the Activities tab is selected.");

                driver.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityAddTask])).ClickWait();;

                this.SetValue(Elements.ElementId[Reference.ActivityFeed.ActivityTaskSubjectId], subject);
                this.SetValue(Elements.ElementId[Reference.ActivityFeed.ActivityTaskDescriptionId], description);
                this.SetCalenderValue(Elements.ElementId[Reference.ActivityFeed.ActivityAddTaskDueDateId], dueDate.ToShortDateString());

                if (driver.ElementExists(By.Id(Elements.ElementId[Reference.ActivityFeed.ActivityAddTaskDueTimeId])))
                {
                    this.SetCalenderValue(Elements.ElementId[Reference.ActivityFeed.ActivityAddTaskDueTimeId], dueDate.ToShortTimeString());
                }

                // Update prioritycode id value for unique declaration in AddTask
                priority.Name = "quickCreateActivity4212controlId_[NAME]_d".Replace("[NAME]", priority.Name);

                this.SetValue(priority);

                driver.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityTaskOk])).ClickWait();;

                return true;
            });
        }

        internal BrowserCommandResult<bool> SetCalenderValue(string field, string value)
        {
            return this.Execute("Set calender value", driver =>
            {
                if (driver.ElementExists(By.Id(field)))
                {
                    driver.WaitUntilAvailable(By.Id(field));

                    var fieldElement = driver.FindElement(By.Id(field)).ClickWait();;

                    fieldElement.FindElement(By.TagName("input")).SendKeysWait(value, true);
                }
                else
                    throw new InvalidOperationException($"Field: {field} Does not exist");

                return true;
            });
        }

        /// <summary>
        /// Adds Email to the activity feed
        /// </summary>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        /// <example>xrmBrowser.ActivityFeed.AddEmail();</example>
        public BrowserCommandResult<bool> AddEmail(int thinkTime = Constants.DefaultThinkTime)
        {
            Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions($"Add Email from Activity Feed"), driver =>
            {
                if (!driver.ElementExists(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityWall])))
                    throw new InvalidOperationException("The Activity Feed is not available. Please check that the Activities tab is selected.");

                driver.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityMoreActivities])).ClickWait();;
                var activitiesList = driver.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityStatusFilterDialog]));
                var appointment = activitiesList.FindElements(By.TagName("li"));
                appointment.Where(x => x.Text.ToLower() == Activities.Email.ToString().ToLower()).FirstOrDefault()?.ClickWait();

                return true;
            });
        }

        /// <summary>
        /// Adds Appointment to the activity feed
        /// </summary>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        /// <example>xrmBrowser.ActivityFeed.AddAppointment();</example>
        public BrowserCommandResult<bool> AddAppointment(int thinkTime = Constants.DefaultThinkTime)
        {
            Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions($"Add Appointment from Activity Feed"), driver =>
            {
                if (!driver.ElementExists(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityWall])))
                    throw new InvalidOperationException("The Activity Feed is not available. Please check that the Activities tab is selected.");

                driver.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityMoreActivities])).ClickWait();;
                var activitiesList = driver.FindElement(By.XPath(Elements.Xpath[Reference.ActivityFeed.ActivityStatusFilterDialog]));

                var appointment = activitiesList.FindElements(By.TagName("li"));
                appointment.Where(x => x.Text.ToLower() == Activities.Appointment.ToString().ToLower()).FirstOrDefault()?.ClickWait();

                return true;
            });
        }
    }
}
