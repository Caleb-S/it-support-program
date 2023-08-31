# IT Support Ticket Management System

This repository contains the code for an IT Support Ticket Management System. The system allows users to submit IT support tickets, which can then be assigned, modified, and managed by administrators. The system is designed to streamline the process of handling IT-related issues within an organization.

## Features

- **User Authentication**: The system provides a login mechanism that distinguishes between students, teachers, and administrators. Users are directed to their respective panels upon login.

- **Ticket Submission**: Students and teachers can create new IT support tickets. They are required to specify the category of the issue, provide a brief summary, and describe the issue in detail.

- **Ticket Management**: Administrators can view all available tickets, assign tickets to other administrators, and filter tickets by assigned staff members.

- **Ticket Modification**: Administrators can modify the category and assigned staff member for a ticket. This can be done through an intuitive interface.

- **Ticket Deletion**: Administrators have the ability to delete tickets from the system when necessary.

## Database Structure

The database is designed with two main tables: `users` and `tickets`. The structure of these tables is as follows:

### Users Table

- `user_id` (INT): Unique identifier for users.
- `password` (VARCHAR): Encrypted password for user login.
- `name` (VARCHAR): Name of the user (student, teacher, or admin).
- `status` (TINYTEXT): User status (student, teacher, or admin).

### Tickets Table

- `ticket_id` (INT): Unique identifier for each support ticket.
- `subject` (VARCHAR): Brief summary of the issue.
- `category` (TINYTEXT): Category of the issue.
- `description` (TEXT): Detailed description of the issue.
- `assigned` (INT): Identifier of the assigned staff member.
- `user_id` (INT): Foreign key linking to the `user_id` in the `users` table.

## Usage

To use the IT Support Ticket Management System, follow these steps:

1. Clone the repository to your local machine.
2. Set up a MySQL database using the provided database structure.
3. Configure the database connection in the code to match your database credentials.
4. Run the application and use the login functionality to access the appropriate panels.
5. Create, manage, assign, modify, and delete tickets based on user roles.

## Conclusion

The IT Support Ticket Management System is a powerful tool for organizations to manage IT-related issues efficiently. By providing distinct user roles, ticket submission, and management capabilities, the system enables seamless communication between users and administrators. This system is designed to improve the overall IT support process and enhance user experience.
