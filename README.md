# File Upload and Management System

This is a simple File Upload and Management System built using the ASP.NET Core MVC template. The application allows users to upload files, save them securely on the server, and display the uploaded files in a user-friendly interface.

## Features

- **File Upload Interface**:
  - Drag-and-drop functionality for uploading files.
  - "Choose File" button as an alternative.
  - Input field for providing a description of the uploaded file.

- **Validation**:
  - Only specific file types are allowed: `.pdf`, `.png`, `.jpg`, `.docx`.
  - Maximum file size limit: **5MB**.
  - A description is required when uploading a file.

- **File Saving**:
  - Files are saved in a designated `uploads` directory on the server.
  - Files are assigned a unique name using `Guid.NewGuid().ToString()` to prevent naming conflicts.
  - The original file name is stored in the database for user reference.

- **Database Integration**:
  - File metadata, including the original file name, description, and unique identifier, is stored in the database.

- **User Feedback**:
  - Success messages displayed on successful uploads.
  - Error messages displayed for validation failures or unexpected errors.

- **File Display**:
  - Uploaded files are shown in a table with their filename and description.
  - File names are clickable links that allow users to download or view the files with their original names.

- **Security**:
  - File paths are secured to prevent manipulation.
  - Malicious file execution is prevented through file name sanitization and strict validation of allowed file types.
