# RaceDay

## Project Overview

RaceDay is a web-based race event management system designed for the South African road running, walking and cycling community.

The system is designed to support two main user roles: **Organisers** and **Participants**. Organisers can create and manage race events, categories and participant results, while Participants can browse available events, enter races, view their enrolments and track their results.

Part One of the project focuses on the planning and database foundation of the RaceDay system. This includes the Entity Relationship Diagram (ERD), API endpoint plan and SQL database script. These artefacts provide the design foundation for the API and MVC application that will be developed in later parts of the project.

---

## Features

### Organiser Features

* Create race events
* Update race events
* Delete race events
* Manage event categories
* View event enrolments
* Capture and manage participant results

### Participant Features

* Create and manage an account
* Browse available race events
* View event information
* Enter race events
* View personal event enrolments
* View personal race results and performance history

### Part One Features

* Logical database design
* Entity Relationship Diagram
* REST API endpoint planning
* SQL Server database schema
* Seed/sample data
* Database constraints and relationships
* GitHub version control
* CI/CD validation using GitHub Actions

---

## Technology Stack

| Technology                          | Purpose                                           |
| ----------------------------------- | ------------------------------------------------- |
| C#                                  | Application development                           |
| .NET / ASP.NET Core                 | Backend and API development                       |
| SQL Server                          | Relational database                               |
| SQL Server Management Studio (SSMS) | Database development and testing                  |
| REST API                            | Communication between application components      |
| GitHub                              | Source control and project repository             |
| GitHub Actions                      | CI/CD and automated validation                    |
| MVC                                 | Web application architecture                      |
| Docker                              | Application containerisation in later development |
| Microsoft Azure                     | Cloud services in later development               |

---

## Repository Structure

```text
RaceDay/
│
├── docs/
│   ├── API/
│   │   └── API_Endpoint_Plan.xlsx
│   │
│   ├── ERD/
│   │   └── RaceDay-ERD.png
│   │
│   └── SQL/
│       └── RaceDayDatabase.sql
│
├── .github/
│   └── workflows/
│       └── ...
│
└── README.md
```

The `docs` directory contains the main Part One documentation and database artefacts.

---

## Part One Documentation

### ERD

The Entity Relationship Diagram is available at:

[`docs/ERD/RaceDay-ERD.png`](docs/ERD/RaceDay-ERD.png)

The ERD represents the RaceDay database entities, attributes, primary keys, foreign keys, relationships and multiplicities.

### API Plan

The API endpoint plan is available at:

[`docs/API/API_Endpoint_Plan.xlsx`](docs/API/API_Endpoint_Plan.xlsx)

The plan defines the REST API endpoints required by the system, including the HTTP methods, routes, purposes, roles and expected request/response information.

The planned API areas include:

* Authentication
* User Profiles
* Events
* Categories
* Event Enrolments
* Results

### SQL Database Script

The database script is available at:

[`docs/SQL/RaceDayDatabase.sql`](docs/SQL/RaceDayDatabase.sql)

The script contains the SQL required to create the RaceDay database structure and populate it with sample data.

---

## Reproducing the Part One Setup

Another developer should be able to reproduce the Part One setup using the files contained in this repository.

### Requirements

Before starting, install or have access to:

* Git
* GitHub account
* SQL Server
* SQL Server Management Studio (SSMS)

### Step 1 – Clone the Repository

Clone the repository from GitHub:

```bash
git clone <REPOSITORY-URL>
```

Navigate into the project directory:

```bash
cd RaceDay
```

### Step 2 – Verify the Documentation

Confirm that the following files exist:

```text
docs/
├── API/
│   └── API_Endpoint_Plan.xlsx
├── ERD/
│   └── RaceDay-ERD.png
└── SQL/
    └── RaceDayDatabase.sql
```

### Step 3 – Review the ERD

Open:

```text
docs/ERD/RaceDay-ERD.png
```

Use the ERD to understand the database entities and relationships before executing the SQL script.

### Step 4 – Set Up the Database

Open **SQL Server Management Studio**.

1. Connect to the required SQL Server instance.
2. Open the file:

```text
docs/SQL/RaceDayDatabase.sql
```

3. Review the script.
4. Execute the script.
5. Confirm that the database and required tables have been created.
6. Confirm that the supplied seed/sample data has been inserted successfully.

### Step 5 – Verify the Database

After execution, verify that:

* All expected tables exist.
* Primary keys have been created.
* Foreign key relationships have been created.
* Constraints are present.
* Seed data has been inserted.
* The resulting database structure corresponds with the ERD.

### Step 6 – Review the API Plan

Open:

```text
docs/API/API_Endpoint_Plan.xlsx
```

Review the planned API endpoints and confirm that they correspond with the system requirements and database design.

This completes the reproducible **Part One setup**.

---

## SQL Instructions

The SQL script should be executed on a clean SQL Server instance or test database environment.

The recommended process is:

1. Open SQL Server Management Studio.
2. Connect to SQL Server.
3. Open `docs/SQL/RaceDayDatabase.sql`.
4. Execute the complete script.
5. Check for execution errors.
6. Refresh the database list and verify that the RaceDay database has been created.
7. Inspect the tables and relationships.
8. Query the tables to confirm that the seed data has been inserted.

The SQL implementation should remain consistent with the final ERD.

---

## CI/CD Status

GitHub Actions is used to provide automated validation of the RaceDay repository.

The CI/CD workflow is intended to verify that the required project structure and Part One documentation are present and that the repository can be validated consistently.

### Current Status

**CI/CD: In Progress**

The final README will include evidence of the successful workflow once the GitHub Actions workflow has completed successfully.

**Successful Build Evidence:**
*To be added after the first successful green CI/CD build.*

---

## Future Development

The RaceDay project will continue beyond Part One.

### Part Two – REST API

The planned API will be implemented using ASP.NET Core and will provide functionality for:

* Authentication
* User profiles
* Events
* Categories
* Event enrolments
* Results

### Part Three – MVC Application

The web application will provide the user-facing interface for Organisers and Participants.

Further planned development includes:

* MVC implementation
* Role-based functionality
* Azure Blob Storage
* Docker/containerisation
* Cloud deployment
* Integration between the MVC application, API and database
  

**YouTube Link:**
*To be added once the presentation has been uploaded.*




