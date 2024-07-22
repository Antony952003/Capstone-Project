# Employee Grievance Redressal API Documentation

## Class Diagram

### User

- **Attributes:**
  - `id`: string (Unique identifier for the user)
  - `name`: string (Full name of the user)
  - `phone`: string (Phone number of the user)
  - `email`: string (Email address of the user)
  - `userImage`: string (URL or path to the user's profile image)
  - `dob`: string (Date of birth in ISO format)
  - `role`: string (Role of the user, e.g., 'admin', 'employee', 'solver')
  - `status`: string (Status of the user, e.g., 'approved', 'pending', 'active', 'inactive')

### Grievance

- **Attributes:**
  - `id`: string (Unique identifier for the grievance)
  - `employeeId`: string (ID of the employee who raised the grievance)
  - `description`: string (Description of the grievance)
  - `priority`: string (Priority level of the grievance, e.g., 'Low', 'Medium', 'High')
  - `status`: string (Current status of the grievance, e.g., 'Open', 'Resolved', 'Closed')
  - `type`: string (Type of grievance, e.g., 'Harassment', 'Work Environment')
  - `documents`: List<string> (List of document URLs related to the grievance)
  - `assignedTo`: string (ID of the solver or admin assigned to the grievance)
  - `createdDate`: string (Date the grievance was created)
  - `updatedDate`: string (Date the grievance was last updated)
  - `resolvedDate`: string (Date the grievance was resolved)

### Solution

- **Attributes:**
  - `id`: string (Unique identifier for the solution)
  - `grievanceId`: string (ID of the grievance the solution is for)
  - `description`: string (Description of the solution)
  - `documents`: List<string> (List of document URLs related to the solution)
  - `providedBy`: string (ID of the admin or solver who provided the solution)
  - `status`: string (Status of the solution, e.g., 'Pending', 'Accepted', 'Rejected')
  - `createdDate`: string (Date the solution was created)
  - `updatedDate`: string (Date the solution was last updated)

### Feedback

- **Attributes:**
  - `id`: string (Unique identifier for the feedback)
  - `solutionId`: string (ID of the solution the feedback is for)
  - `employeeId`: string (ID of the employee providing the feedback)
  - `rating`: int (Rating given to the solution)
  - `comments`: string (Comments provided by the employee)
  - `createdDate`: string (Date the feedback was created)

### GrievanceHistory

- **Attributes:**
  - `id`: string (Unique identifier for the history entry)
  - `grievanceId`: string (ID of the grievance this history entry is related to)
  - `action`: string (Action taken, e.g., 'Assigned', 'Resolved', 'Forwarded')
  - `userId`: string (ID of the user who performed the action)
  - `timestamp`: string (Date and time when the action was performed)
  - `sequenceNumber`: int (Order in which the action occurred)
  - `details`: string (Additional details about the action)

### Admin

- **Attributes:**
  - Inherits attributes from `User`
- **Methods:**
  - `approveEmployee(employeeId: string): void`
  - `assignRole(userId: string, role: string): void`
  - `getEmployeeProfile(employeeId: string): User`
  - `getAllComplaints(): List<Grievance>`
  - `getEmployeeComplaints(employeeId: string): List<Grievance>`
  - `assignGrievance(grievanceId: string, solverId: string): void`
  - `approveRequest(requestId: string): void`
  - `getGrievanceReports(): List<Report>`
  - `forwardGrievance(grievanceId: string, solverId: string): void`

### Employee

- **Attributes:**
  - Inherits attributes from `User`
- **Methods:**
  - `createGrievance(description: string, priority: string, type: string, documents: List<string>): void`
  - `getGrievance(grievanceId: string): Grievance`
  - `updateGrievance(grievanceId: string, description: string, priority: string, type: string, documents: List<string]): void`
  - `deleteGrievance(grievanceId: string): void`
  - `getGrievanceHistory(grievanceId: string): List<GrievanceHistory>`
  - `provideFeedback(solutionId: string, rating: int, comments: string): void`
  - `requestApproval(reason: string): void`

### Solver

- **Attributes:**
  - `complaintsSolved`: List<string> (List of grievance IDs solved by the solver)
  - `solverType`: string (Type of solver, e.g., 'Harassment', 'Work Environment')
- **Methods:**
  - `getSolverType(): string`
  - `addSolution(grievanceId: string, description: string, documents: List<string]): void`
  - `getSolution(solutionId: string): Solution`
  - `updateSolution(solutionId: string, description: string, documents: List<string]): void`
  - `deleteSolution(solutionId: string): void`
  - `requestApproval(reason: string): void`
  - `requestForwardGrievance(grievanceId: string, reason: string): void`

### ApprovalRequest

- **Attributes:**
  - `id`: string (Unique identifier for the approval request)
  - `userId`: string (ID of the user making the request)
  - `reason`: string (Reason for the request)
  - `status`: string (Status of the request, e.g., 'Pending', 'Approved', 'Rejected')
  - `createdDate`: string (Date the request was created)
  - `approvedDate`: string (Date the request was approved, if applicable)

### Report

- **Attributes:**
  - `id`: string (Unique identifier for the report)
  - `grievanceId`: string (ID of the grievance the report is related to)
  - `metrics`: List<string> (List of metrics related to the grievance)
  - `createdDate`: string (Date the report was created)

## Relationships

- **User** <|-- **Admin**: Admins are a type of User.
- **User** <|-- **Employee**: Employees are a type of User.
- **User** <|-- **Solver**: Solvers are a type of User.
- **Grievance** --> "1" **User**: `raisedBy` - An employee raises a grievance.
- **Grievance** --> "1" **User**: `assignedTo` - A grievance can be assigned to a user (admin or solver).
- **Grievance** --> "0..\*" **GrievanceHistory**: A grievance can have multiple history entries.
- **GrievanceHistory** --> "1" **Grievance**: `belongsTo` - Each history entry is related to one grievance.
- **GrievanceHistory** --> "1" **User**: `performedBy` - The user who performed the action.
- **Grievance** --> "0..\*" **Solution**: A grievance can have multiple solutions.
- **Solution** --> "1" **Grievance**: `belongsTo` - Each solution is related to one grievance.
- **Solution** --> "1" **User**: `providedBy` - The user who provided the solution.
- **Solution** --> "0..1" **Feedback**: Each solution can have one feedback.
- **Feedback** --> "1" **Solution**: `belongsTo` - Each feedback is related to one solution.
- **Feedback** --> "1" **User**: `providedBy` - The user who provided the feedback.
- **ApprovalRequest** --> "1" **User**: `requestedBy` - The user who made the approval request.
- **ApprovalRequest** --> "1" **User**: `approvedBy` - The user who approved the request.
- **Report** --> "1" **Grievance**: `generatedFor` - Each report is related to one grievance.

## API Endpoints

### User Management

#### Employee Registration

- **POST** `/api/employees/register`
- **Purpose:** Register a new employee with details such as name, phone, email, user image, DOB, and role.

#### Employee Login

- **POST** `/api/employees/login`
- **Purpose:** Authenticate an employee and issue a token for access to the application.

#### Approve Employee

- **POST** `/api/employees/{id}/approve`
- **Purpose:** Approve an employee's registration, allowing them to raise grievances.

#### Get Employee Profile

- **GET** `/api/employees/{id}`
- **Purpose:** Retrieve details of a specific employee.

#### Update Employee Profile

- **PUT** `/api/employees/{id}`
- **Purpose:** Update details of an employee, including name, phone, email, etc.

#### Assign Role

- **PUT** `/api/employees/{id}/role`
- **Purpose:** Assign or update the role of a user (admin, employee, or solver).

### Grievance Management

#### Create Grievance

- **POST** `/api/grievances`
- **Purpose:** Submit a new grievance, including description, priority, documents, and type.

#### Get Grievance

- **GET** `/api/grievances/{id}`
- **Purpose:** Retrieve details of a specific grievance.

#### Update Grievance

- **PUT** `/api/grievances/{id}`
- **Purpose:** Update details of an existing grievance, including description, priority, documents, and type.

#### Delete Grievance

- **DELETE** `/api/grievances/{id}`
- **Purpose:** Remove a grievance from the system.

#### Forward Grievance

- **POST** `/api/grievances/{id}/forward`
- **Purpose:** Request to forward a grievance to an admin if it cannot be resolved. The admin will handle the forwarding to another solver if needed.

#### Get Forwarded Grievances

- **GET** `/api/grievances/forwarded`
- **Purpose:** Retrieve a list of grievances that have been forwarded to the current admin.

#### Get Grievance History

- **GET** `/api/grievances/{id}/history`
- **Purpose:** Track and retrieve the history of actions taken on a specific grievance.

#### Assign Grievance

- **PUT** `/api/grievances/{id}/assign`
- **Request Body:**
  ```json
  {
    "solverId": "string"
  }
  ```

## Solutions and Feedback

### Add Solution

- **POST** `/api/solutions`
- **Purpose:** Submit a solution for a grievance, including description and optional documents.

### Get Solution

- **GET** `/api/solutions/{id}`
- **Purpose:** Retrieve details of a specific solution.

### Update Solution

- **PUT** `/api/solutions/{id}`
- **Purpose:** Update details of an existing solution, including description and optional documents.

### Delete Solution

- **DELETE** `/api/solutions/{id}`
- **Purpose:** Remove a solution from the system.

### Provide Feedback

- **POST** `/api/solutions/{id}/feedback`
- **Purpose:** Submit feedback and a rating for a specific solution provided by an admin.

## Admin Management

### Get All Complaints

- **GET** `/api/complaints`
- **Purpose:** Retrieve a list of all grievances in the system.

### Get Employee Complaints

- **GET** `/api/employees/{id}/complaints`
- **Purpose:** Retrieve a list of all grievances raised by a specific employee.

### Review and Approve Employee Profile

- **PUT** `/api/employees/{id}/approve`
- **Purpose:** Review and approve an employee's profile, allowing them to raise grievances.

### Get Reports on Grievances

- **GET** `/api/reports/grievances`
- **Purpose:** Generate and retrieve reports on grievances, including metrics like resolution times and feedback trends.
