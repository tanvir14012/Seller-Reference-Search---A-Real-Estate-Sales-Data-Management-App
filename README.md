# Seller Reference Search - A Real Estate Sales Data Management App

# Real Estate Sales Data Management App  

A robust web application for managing real estate sales data, designed to handle large-scale uploads and advanced data operations efficiently.  

## 🚀 Features  

### 1. **Asynchronous Excel Uploads**  
- Built with **multithreading** for high performance.  
- Utilizes a **background job and process queue** for seamless Excel file uploads without interrupting the user experience.  

### 2. **Advanced Data Parsing and Storage**  
- Parses uploaded Excel files to extract structured real estate sales data.  
- Stores data in a secure and highly performant **PostgreSQL 16** database.  

### 3. **Search, Filter, and Export**  
- Intuitive search functionality to filter records by:  
  - Lot acreage  
  - Offer price  
  - State  
  - County  
  - Reference/owner name  
- Supports **sorting**, **pagination**, and **data export** for filtered results.  

### 4. **Admin Controls**  
- Manage uploads and delete unnecessary data.  
- Create, update, and manage user accounts with ease.  

---

## 🛠️ Tech Stack  

- **Framework**: [ASP.NET 8 MVC](https://dotnet.microsoft.com/en-us/apps/aspnet/mvc)  
- **Database**: [PostgreSQL 16](https://www.postgresql.org/)  
- **Hosting**: [AWS Lightsail](https://aws.amazon.com/lightsail/) (Ubuntu 22.04)  
- **Web Server**: [Nginx](https://www.nginx.com/)  
- **Security**: SSL encryption for secure communication  

---

## 🌟 Highlights  

- **High Performance**: Asynchronous operations ensure smooth and fast uploads, even with large datasets.  
- **Scalability**: Designed to handle growing data needs effortlessly.  
- **User-Friendly**: Simple and intuitive interface for both general users and administrators.  
- **Secure**: End-to-end security with HTTPS via SSL.  

---

## 📦 How to Set Up  

### Prerequisites  
- AWS Lightsail instance with Ubuntu 22.04/Windows/Local machine  
- PostgreSQL 16 installed and configured  
- Nginx installed as a reverse proxy  
- .NET 8 SDK installed  

### Steps  

1. **Clone the Repository**  

2. **Set Up the Database**  
   - Create a PostgreSQL database.  
   - Run migrations using the Entity Framework CLI:  
     ```bash  
     dotnet ef database update  
     ```  

3. **Configure Environment Variables**  
   - Add environment-specific settings for database connection and security in `appsettings.json`.  

4. **Run the Application**  
   ```bash  
   dotnet run  
   ```  

5. **Set Up Nginx**  
   - Configure Nginx as a reverse proxy for the application.  
   - Ensure SSL is set up for HTTPS.  

---

## 🤝 Contributing  

Contributions are welcome! Please fork the repository and submit a pull request with your changes.  

---

## 📜 License  

This project is licensed under the MIT License. See the `LICENSE` file for details.  

---

## 📧 Contact  

For queries or support, reach out via [tanvir.mbstucs@gmail.com](mailto:tanvir.mbstucs@gmail.com).