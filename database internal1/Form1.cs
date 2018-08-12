using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace database_internal1
{
    public partial class supportForm1 : Form
    {
        private MySqlConnection connection;
        public supportForm1()
        {
            InitializeComponent();
            // Connect to MySql server.
            string connectionString = string.Format("Server=nimbus.rangitoto.school.nz;" +
                "Port=3307;" +
                "database={0};" +
                "UID=2018110394;" +
                "password=110394;" +
                "SslMode=none", "student2018110394"); 
            connection = new MySqlConnection(connectionString);

            try
            {
                // Try and open connection and load listviews.
                connection.Open();                  

            } catch (Exception ex)
            {
                // Notify the user that a connection couldnt be made, display error message and disable login button.
                MessageBox.Show("Cannot connect to db: " + ex.ToString());
                loginLbl1.Text = "Connection failure: reopen application";
                loginBtn.Enabled = false; 
            }
        }

        private void ViewAllLV()
        {
            // Clear items in listview before updating list.
            viewAllLV.Items.Clear();
            // The Sql command reads ticket_id, subject, category and assigned from the tickets table of database.
            var cmd = new MySqlCommand("SELECT ticket_id, subject, category, assigned FROM tickets;", connection);
            // Variables
            List<String> rows = new List<String>();
            // Execute reader on cmd
            var reader = cmd.ExecuteReader();
            // Reads and displays data on seperate rows.
            while (reader.Read())
            {
                List<string> row = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    try
                    {
                        // Add string from reader.
                        row.Add(reader.GetString(i));
                    }
                    catch (Exception)
                    {
                        // If the string is empty(null) add to the row "None".
                        row.Add("None");
                    }
                }
                viewAllLV.Items.Add(new ListViewItem(row.ToArray()));
            }
            // Close the reader.
            reader.Close(); 
        }

        private void UnAssignedLV()
        {
            // Clear items in listview before updating list.
            unAssignedLV.Items.Clear();  
            // The Sql command reads ticket_id, subject, category and assigned where assigned is "null" in tickets table of database.
            var cmd = new MySqlCommand("SELECT ticket_id, subject, category, assigned FROM tickets WHERE assigned IS null;", connection);
            // Variables.
            List<String> rows = new List<String>();
            // Execute reader on cmd.
            var reader = cmd.ExecuteReader();
            // Reads and displays data on seperate rows.
            while (reader.Read())
            {
                List<string> row = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    try
                    {
                        // Add string from reader.
                        row.Add(reader.GetString(i));
                    }
                    catch (Exception)
                    {
                        // If the string is empty(null) add to the row "None".
                        row.Add("None");
                    }
                }
                unAssignedLV.Items.Add(new ListViewItem(row.ToArray()));
            }
            // Close the reader.
            reader.Close(); 
        }

        private void MyAssignedLV()
        {
            // Clear items in listview before updating list.
            myAssignedLV.Items.Clear();
            // The Sql command reads ticket_id, subject, category and assigned where assigned is the admins id in tickets table of database.
            var cmd = new MySqlCommand("SELECT ticket_id, subject, category, assigned FROM tickets WHERE assigned = @user;", connection);
            // Variables.
            List<String> rows = new List<String>();
            // Execute reader on cmd.
            cmd.Parameters.AddWithValue("@user", usernameTxtBx.Text);
            var reader = cmd.ExecuteReader();
            // Reads and displays data on seperate rows.
            while (reader.Read())
            {
                List<string> row = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    try
                    {
                        // Add string from reader.
                        row.Add(reader.GetString(i));
                    }
                    catch (Exception)
                    {
                        // If the string is empty(null) add to the row "None".
                        row.Add("None");
                    }
                }
                myAssignedLV.Items.Add(new ListViewItem(row.ToArray()));
            }
            // Closes the reader.
            reader.Close(); 

        }      

        private void loginBtn_Click(object sender, EventArgs e)
        {
            // Logincmd checks if the username or password enter matchs any inside users table of database.
            // Statuscmd returns the status that matchs the username in the users table of database.
            var loginCmd = new MySqlCommand(string.Format("SELECT user_id FROM users WHERE user_id = @username AND password = SHA2('{0}', 256);", passwordTxtBx.Text), connection);
            var statusCmd = new MySqlCommand(string.Format("SELECT status FROM users WHERE user_id = @username;"), connection);
            // Variables.
            loginCmd.Parameters.AddWithValue("@username", usernameTxtBx.Text);
            statusCmd.Parameters.AddWithValue("@username", usernameTxtBx.Text);
            String user = null; 
            String status = (string)statusCmd.ExecuteScalar(); 
            try
            {
                // Try execute the user command.
                user = loginCmd.ExecuteScalar().ToString();                  
            }
            catch(Exception)
            {
                // If user command couldnt be executed the username or password was wrong.
                loginLbl1.Text = "The username or password was incorrect";
            }

            // Check if user string is no longer null (if its not null the username and password were correct).
            if (!String.IsNullOrEmpty(user))
            {
                // Check if the user is a admin.
                if (status == "admin")
                {           
                    mainStkP.SelectTab("adminTb");
                    // Load all the listviews when an admin logs in.
                    MyAssignedLV();
                    ViewAllLV();
                    UnAssignedLV();  
                }
                else 
                {
                    // If the user is anything but a admin they are displayed the feedback screen.
                    mainStkP.SelectTab("feedbackTb");                                 
                }
                // Reset the login label.
                loginLbl1.Text = ""; 
            }           
        }

        private void DescriptionTxtBx_Enter(object sender, EventArgs e)
        {
            // If the user selects the DescriptionTxtBx and it is equal to default text, the text box is cleared.
            if (descriptionTxtBx.Text == "Please include:\n - A clear description of the problem \n - A step by step of instr" +
                "uctions to reproduce the problem(if possible)\n - What results you expect\n - What" +
                " results you actually saw")
            {
                // Sets the textbox to an empty string.
                descriptionTxtBx.Text = ""; 
            }
        }

        private void logoutbtn2_Click(object sender, EventArgs e)
        {
            // Set the comboboxs when modifying to null. 
            changeAssignedCBx.SelectedItem = null;
            changeCategoryCBx.SelectedItem = null;
            // Set all labels of view ticket back to "Unknown".
            displaySubjectLbl2.Text = "Unknown";
            displayTxtBx2.Text = "Unknown";
            displayFromLbl2.Text = "Unknown";
            displayCategoryLbl.Text = "Unknown";
            displayAssignedLbl.Text = "Unknown";
            displaySubjectLbl1.Text = "Unknown";
            displayTxtBx1.Text = "Unknown";
            displayFromLbl1.Text = "Unknown";
            // Set list view column widths back to default.
            // --viewAllLV--
            ticketId.Width = 80;
            subject.Width = 80;
            category.Width = 80;
            assigned.Width = 80;
            // --unAssignedLV--
            ticketId2.Width = 80;
            subject2.Width = 80;
            category2.Width = 80;
            assigned2.Width = 80;
            // --myAssignedLV--
            ticketId3.Width = 80;
            subject3.Width = 80;
            category3.Width = 80;
            assigned3.Width = 80;
            // Clear login text boxs and listviews.
            usernameTxtBx.Clear();
            passwordTxtBx.Clear();
            viewAllLV.SelectedItems.Clear();
            unAssignedLV.SelectedItems.Clear();
            myAssignedLV.SelectedItems.Clear();
            // Set all tabs back to default and change to login page.
            mainStkP.SelectTab("loginTb");
            ticketTbControl.SelectTab("ticketViewTb");
            adminStkP.SelectTab("viewTicketTb");
        }

        private void logoutBtn1_Click(object sender, EventArgs e)
        {
            // Set everythng back to defualt
            titleTxtBx.Text = "";
            categoryCBx.SelectedIndex = -1;            
            descLbl1.ForeColor = Color.Black;
            descLbl1.Text = "Please provide a decriptive title for your feedback:";
            descLbl2.ForeColor = Color.Black;
            descLbl2.Text = "Choose what type of issue your reporting:";
            descLbl3.ForeColor = Color.Black;
            descLbl3.Text = "Please describe the issue and what steps we can take to reproduce it:";
            descriptionTxtBx.Text = "Please include:\n - A clear description of the problem \n - A step by step of instr" +
                "uctions to reproduce the problem(if possible)\n - What results you expect\n - What" +
                " results you actually saw";
            // Clear login text boxs
            usernameTxtBx.Clear();
            passwordTxtBx.Clear();
            // Change to login page.
            mainStkP.SelectTab("loginTb");
        }

        private void modifyBtn1_Click(object sender, EventArgs e)
        {
            // If an item is selected from a listview change to modifyTb in adminStkP.
            if (viewAllLV.SelectedItems.Count > 0 || unAssignedLV.SelectedItems.Count > 0 || myAssignedLV.SelectedItems.Count > 0)
            {
                adminStkP.SelectTab("modifyTb");
            }
            // MySqlCommand cmd selects name and userid in users table if status is admin.
            MySqlCommand cmd = new MySqlCommand("SELECT name, user_id FROM users WHERE status = 'admin';", connection);
            // Execute reader storing as admins.
            MySqlDataReader admins = cmd.ExecuteReader();

            // Reads admins and adds as items to combobox.
            if (changeAssignedCBx.Items.Count == 0)
            {
                while (admins.Read())
                {                   
                    changeAssignedCBx.Items.Add(String.Format("({1}) {0}", admins[0], admins[1]));
                }
            }        
            // Close the reader admins.
            admins.Close();
        }

        private void backBtn1_Click(object sender, EventArgs e)
        {
            // Change back to viewTicketTb in admin stackpanel
            adminStkP.SelectTab("viewTicketTb");
        }     

        private void submitBtn1_Click(object sender, EventArgs e)
        {
            // Command adds the entered feedback to the table tickets in database.
            var cmd = new MySqlCommand("INSERT INTO tickets (`subject`, `category`, `description`, `user_id`) VALUES (@subject, @category, @description, @user);", connection);
            // Variables.
            cmd.Parameters.AddWithValue("@subject", titleTxtBx.Text);
            cmd.Parameters.AddWithValue("@category", categoryCBx.Text);
            cmd.Parameters.AddWithValue("@description", descriptionTxtBx.Text);
            cmd.Parameters.AddWithValue("@user", usernameTxtBx.Text);
            // Check if any field is empty/unchanged.
            if (titleTxtBx.Text == "" || categoryCBx.Text == "" || descriptionTxtBx.Text == "" || descriptionTxtBx.Text == "Please include:\n - A clear description of the problem " +
                "\n - A step by step of instr" +
                "uctions to reproduce the problem(if possible)\n - What results you expect\n - What" +
                " results you actually saw")
            {
                // Check if titleTxtBx is empty.
                if (titleTxtBx.Text == "")
                {
                    descLbl1.ForeColor = Color.Red;
                    descLbl1.Text = "Required - Please provide a decriptive title for your feedback:";
                }
                else if (titleTxtBx.Text != "")
                {
                    descLbl1.ForeColor = Color.Black;
                    descLbl1.Text = "Please provide a decriptive title for your feedback:";
                }
                // Check if categoryCbx is empty
                if (categoryCBx.Text == "")
                {
                    descLbl2.ForeColor = Color.Red;
                    descLbl2.Text = "Required - Choose what type of issue your reporting:";
                }
                else if (categoryCBx.Text != "")
                {
                    descLbl2.ForeColor = Color.Black;
                    descLbl2.Text = "Choose what type of issue your reporting:";
                }
                // Check if decriptionTxtBx is empty or as default.
                if (descriptionTxtBx.Text == "" || descriptionTxtBx.Text == "Please include:\n - A clear description of the problem \n - A step by step of instr" +
                    "uctions to reproduce the problem(if possible)\n - What results you expect\n - What" +
                    " results you actually saw")
                {
                    descLbl3.ForeColor = Color.Red;
                    descLbl3.Text = "Required - Please describe the issue and what steps we can take to reproduce it:";
                    descriptionTxtBx.Text = ("Please include:\n - A clear description of the problem \n - A step by step of instr" +
                        "uctions to reproduce the problem(if possible)\n - What results you expect\n - What" +
                        " results you actually saw");
                }
                else if (descriptionTxtBx.Text != "")
                {
                    descLbl3.ForeColor = Color.Black;
                    descLbl3.Text = "Please describe the issue and what steps we can take to reproduce it:";
                }
            }
            else
            {
                // If no fields are empty Excute the command.
                var feedback = cmd.ExecuteScalar();
                // Set all fields back to default.
                titleTxtBx.Text = "";
                categoryCBx.SelectedIndex = -1;
                descLbl1.ForeColor = Color.Black;
                descLbl1.Text = "Please provide a decriptive title for your feedback:";
                descLbl2.ForeColor = Color.Black;
                descLbl2.Text = "Choose what type of issue your reporting:";
                descLbl3.ForeColor = Color.Black;
                descLbl3.Text = "Please describe the issue and what steps we can take to reproduce it:";
                descriptionTxtBx.Text = "Please include:\n - A clear description of the problem \n - A step by step of instr" +
                    "uctions to reproduce the problem(if possible)\n - What results you expect\n - What" +
                    " results you actually saw";
            } 
        }

        private void ticketTbControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ticketTbControl.SelectedTab == ticketTbControl.TabPages["ticketViewTb"])
            {
                // Deselect items in all listviews otherthan viewAllLV.
                unAssignedLV.SelectedItems.Clear();
                myAssignedLV.SelectedItems.Clear();              
            }
            if (ticketTbControl.SelectedTab == ticketTbControl.TabPages["unassignedTb"])
            {
                // Deselect items in all listviews otherthan unassigndLV.
                viewAllLV.SelectedItems.Clear();
                myAssignedLV.SelectedItems.Clear();       
            }
            if (ticketTbControl.SelectedTab == ticketTbControl.TabPages["assignedTb"])
            {
                // Deselect items in all listviews otherthan myAssignedLV.
                viewAllLV.SelectedItems.Clear();
                unAssignedLV.SelectedItems.Clear();           
            }
            // Set comboboxs back to null.
            changeAssignedCBx.SelectedItem = null;
            changeCategoryCBx.SelectedItem = null;
            // Set all labels back to "Unknown".
            displaySubjectLbl2.Text = "Unknown";
            displayTxtBx2.Text = "Unknown";
            displayFromLbl2.Text = "Unknown";
            displayCategoryLbl.Text = "Unknown";
            displayAssignedLbl.Text = "Unknown";
            displaySubjectLbl1.Text = "Unknown";
            displayTxtBx1.Text = "Unknown";
            displayFromLbl1.Text = "Unknown";
            // Change admin stackpanel back to viewTicketTb (reset back to default).
            adminStkP.SelectTab("viewTicketTb");
        }  

        private void saveBtn2_Click(object sender, EventArgs e)
        {
            // Sql command updates assigned and category in tickets table for the selected ticket.
            var cmd = new MySqlCommand("UPDATE tickets SET assigned = @assign, category = @category WHERE ticket_id = @ticket;" , connection);
            // Variables
            try
            {
                // Takes the number inbetween the brackets displayed in combobox that is the admins id (111014).
                cmd.Parameters.AddWithValue("@assign", ("" + changeAssignedCBx.SelectedItem).Substring(1, 6));
            }
            catch
            {
                // If there is no number theres probably nothing selected so set @assign to whats in combo box.
                cmd.Parameters.AddWithValue("@assign", changeAssignedCBx.SelectedItem);
            }                    
            cmd.Parameters.AddWithValue("@category", changeCategoryCBx.SelectedItem);
            // Check if an item is selected from any of the three listviews
            if (viewAllLV.SelectedItems.Count > 0 || unAssignedLV.SelectedItems.Count > 0 || myAssignedLV.SelectedItems.Count > 0)
            {
                if (viewAllLV.SelectedItems.Count > 0 )
                {
                    // Gets ticket id from the selected data from viewAllLV setting it to @ticket parameter.
                    cmd.Parameters.AddWithValue("@ticket", viewAllLV.SelectedItems[0].SubItems[0].Text);                               
                }
                if (unAssignedLV.SelectedItems.Count > 0)
                {
                    // Gets ticket id from the selected data from unAssignedLV setting it to @ticket parameter.
                    cmd.Parameters.AddWithValue("@ticket", unAssignedLV.SelectedItems[0].SubItems[0].Text);               
                }
                if (myAssignedLV.SelectedItems.Count > 0)
                {
                    // Gets ticket id from the selected data from myAssignedLV setting it to @ticket parameter.
                    cmd.Parameters.AddWithValue("@ticket", myAssignedLV.SelectedItems[0].SubItems[0].Text);
                }
                // Execute cmd - updating the data in database. 
                cmd.ExecuteNonQuery();
                // Reload all listviews.
                ViewAllLV();
                UnAssignedLV();
                MyAssignedLV();
                // Set comboboxs back to null.
                changeAssignedCBx.SelectedItem = null;
                changeCategoryCBx.SelectedItem = null;
                // Set all labels back to "Unknown".
                displaySubjectLbl2.Text = "Unknown";
                displayTxtBx2.Text = "Unknown";
                displayFromLbl2.Text = "Unknown";
                displayCategoryLbl.Text = "Unknown";
                displayAssignedLbl.Text = "Unknown";
                displaySubjectLbl1.Text = "Unknown";
                displayTxtBx1.Text = "Unknown";
                displayFromLbl1.Text = "Unknown";
                // Set admin stackpanel back to viewTicketTb.
                adminStkP.SelectTab("viewTicketTb");             
            }           
        }

        private void viewAllLV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (viewAllLV.SelectedItems.Count > 0)
            {
                // The Sql command "userCmd" selects name in users table where in tickets the ticket_id equal to the @ticket parameter and user_id of tickets equal to user id of users table.
                // The Sql command "descCmd" selects decription from tickets table where ticket id is equal to @ticket parameter.
                var userCmd = new MySqlCommand("SELECT users.name FROM tickets, users WHERE ticket_id = @ticket AND tickets.user_id = users.user_id;", connection);
                var descCmd = new MySqlCommand("SELECT description FROM tickets WHERE ticket_id = @ticket;", connection);
                // Variables.
                string subject = viewAllLV.SelectedItems[0].SubItems[1].Text;
                string category = viewAllLV.SelectedItems[0].SubItems[2].Text;
                string assigned = viewAllLV.SelectedItems[0].SubItems[3].Text;
                userCmd.Parameters.AddWithValue("@ticket", viewAllLV.SelectedItems[0].SubItems[0].Text);
                descCmd.Parameters.AddWithValue("@ticket", viewAllLV.SelectedItems[0].SubItems[0].Text);
                // Execute userCmd and descCmd saving them as user and desc.
                var user = userCmd.ExecuteScalar();
                var desc = descCmd.ExecuteScalar();
                // Change labels and combobox text to above variables.
                displaySubjectLbl1.Text = subject;
                displaySubjectLbl2.Text = subject;
                displayCategoryLbl.Text = category;
                changeCategoryCBx.Text = category;
                displayAssignedLbl.Text = assigned;
                changeAssignedCBx.Text = assigned;
                displayFromLbl1.Text = "" + user;
                displayFromLbl2.Text = "" + user;
                displayTxtBx1.Text = "" + desc;
                displayTxtBx2.Text = "" + desc;
            }
        }

        private void unAssignedLV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (unAssignedLV.SelectedItems.Count > 0)
            {
                // The Sql command "userCmd" selects name in users table where in tickets the ticket_id equal to the @ticket parameter and user_id of tickets equal to user id of users table.
                // The Sql command "descCmd" selects decription from tickets table where ticket id is equal to @ticket parameter.
                var userCmd = new MySqlCommand("SELECT users.name FROM tickets, users WHERE ticket_id = @ticket AND tickets.user_id = users.user_id;", connection);
                var descCmd = new MySqlCommand("SELECT description FROM tickets WHERE ticket_id = @ticket;", connection);
                // Variables.
                string subject = unAssignedLV.SelectedItems[0].SubItems[1].Text;
                string category = unAssignedLV.SelectedItems[0].SubItems[2].Text;
                string assigned = unAssignedLV.SelectedItems[0].SubItems[3].Text;
                userCmd.Parameters.AddWithValue("@ticket", unAssignedLV.SelectedItems[0].SubItems[0].Text);
                descCmd.Parameters.AddWithValue("@ticket", unAssignedLV.SelectedItems[0].SubItems[0].Text);
                // Execute userCmd and descCmd saving them as user and desc.
                var user = userCmd.ExecuteScalar();
                var desc = descCmd.ExecuteScalar();
                // Change labels and combobox text to above variables.
                displaySubjectLbl1.Text = subject;
                displaySubjectLbl2.Text = subject;
                displayCategoryLbl.Text = category;
                changeCategoryCBx.Text = category;
                displayAssignedLbl.Text = assigned;
                changeAssignedCBx.Text = assigned;
                displayFromLbl1.Text = "" + user;
                displayFromLbl2.Text = "" + user;
                displayTxtBx1.Text = "" + desc;
                displayTxtBx2.Text = "" + desc;
            }
        }

        private void myAssignedLV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (myAssignedLV.SelectedItems.Count > 0)
            {
                // The Sql command "userCmd" selects name in users table where in tickets the ticket_id equal to the @ticket parameter and user_id of tickets equal to user id of users table.
                // The Sql command "descCmd" selects decription from tickets table where ticket id is equal to @ticket parameter.
                var userCmd = new MySqlCommand("SELECT users.name FROM tickets, users WHERE ticket_id = @ticket AND tickets.user_id = users.user_id;", connection); 
                var descCmd = new MySqlCommand("SELECT description FROM tickets WHERE ticket_id = @ticket;", connection);
                // Variables.
                string subject = myAssignedLV.SelectedItems[0].SubItems[1].Text;
                string category = myAssignedLV.SelectedItems[0].SubItems[2].Text;
                string assigned = myAssignedLV.SelectedItems[0].SubItems[3].Text;
                userCmd.Parameters.AddWithValue("@ticket", myAssignedLV.SelectedItems[0].SubItems[0].Text);
                descCmd.Parameters.AddWithValue("@ticket", myAssignedLV.SelectedItems[0].SubItems[0].Text);
                // Execute userCmd and descCmd saving them as user and desc.
                var user = userCmd.ExecuteScalar();
                var desc = descCmd.ExecuteScalar();
                // Change labels and combobox text to above variables.
                displaySubjectLbl1.Text = subject;
                displaySubjectLbl2.Text = subject;
                displayCategoryLbl.Text = category;
                changeCategoryCBx.Text = category;
                displayAssignedLbl.Text = assigned;
                changeAssignedCBx.Text = assigned;
                displayFromLbl1.Text = "" + user;
                displayFromLbl2.Text = "" + user;
                displayTxtBx1.Text = "" + desc;
                displayTxtBx2.Text = "" + desc;
            }
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            // Reload all listviews.
            ViewAllLV();
            UnAssignedLV();
            MyAssignedLV();
            // Set comboboxs back to null.
            changeAssignedCBx.SelectedItem = null;
            changeCategoryCBx.SelectedItem = null;
            // Set labels back to default.
            displaySubjectLbl2.Text = "Unknown";
            displayTxtBx2.Text = "Unknown";
            displayFromLbl2.Text = "Unknown";
            displayCategoryLbl.Text = "Unknown";
            displayAssignedLbl.Text = "Unknown";
            displaySubjectLbl1.Text = "Unknown";
            displayTxtBx1.Text = "Unknown";
            displayFromLbl1.Text = "Unknown";
            // Set list view column widths back to default.
            // --viewAllLV--
            ticketId.Width = 80;
            subject.Width = 80;
            category.Width = 80;
            assigned.Width = 80;
            // --unAssignedLV--
            ticketId2.Width = 80;
            subject2.Width = 80;
            category2.Width = 80;
            assigned2.Width = 80;
            // --myAssignedLV--
            ticketId3.Width = 80;
            subject3.Width = 80;
            category3.Width = 80;
            assigned3.Width = 80;
            // Changes admin stackpanel back to viewTicketTb (resets to default)
            adminStkP.SelectTab("viewTicketTb");
        }

        private void delBtn1_Click(object sender, EventArgs e)
        {
            // "deleteCmd" deletes the data from tickets tables if the parameter @ticket is the same as that datas ticket_id.
            var deleteCmd = new MySqlCommand("DELETE FROM tickets WHERE ticket_id = @ticket;", connection);
            // Check if an item is selected from any of the three listviews
            if (viewAllLV.SelectedItems.Count > 0 || unAssignedLV.SelectedItems.Count > 0 || myAssignedLV.SelectedItems.Count > 0)
            {
                if (viewAllLV.SelectedItems.Count > 0)
                {
                    // Gets ticket id from the selected data from viewAllLV setting it to @ticket parameter.
                    deleteCmd.Parameters.AddWithValue("@ticket", viewAllLV.SelectedItems[0].SubItems[0].Text);
                }
                if (unAssignedLV.SelectedItems.Count > 0)
                {
                    // Gets ticket id from the selected data from unAssignedLV setting it to @ticket parameter.
                    deleteCmd.Parameters.AddWithValue("@ticket", unAssignedLV.SelectedItems[0].SubItems[0].Text);
                }
                if (myAssignedLV.SelectedItems.Count > 0)
                {
                    // Gets ticket id from the selected data from myAssignedLV setting it to @ticket parameter.
                    deleteCmd.Parameters.AddWithValue("@ticket", myAssignedLV.SelectedItems[0].SubItems[0].Text);
                }
                // Run the deleteCmd.
                var delete = deleteCmd.ExecuteScalar();
                // Refresh all listviews
                ViewAllLV();
                UnAssignedLV();
                MyAssignedLV();
                // Set comboboxs back to null.
                changeAssignedCBx.SelectedItem = null;
                changeCategoryCBx.SelectedItem = null;
                // Set all labels back to "Unknown".
                displaySubjectLbl2.Text = "Unknown";
                displayTxtBx2.Text = "Unknown";
                displayFromLbl2.Text = "Unknown";
                displayCategoryLbl.Text = "Unknown";
                displayAssignedLbl.Text = "Unknown";
                displaySubjectLbl1.Text = "Unknown";
                displayTxtBx1.Text = "Unknown";
                displayFromLbl1.Text = "Unknown";
                // Set admin stackpanel back to viewTicketTb.
                adminStkP.SelectTab("viewTicketTb");
            }     
        }
    }
}
