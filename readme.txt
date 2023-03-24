
RoadSideAssistance project is built using ASP.Net Core 6 in Visual Studio 2022(Community Edition)
The project is developed as .Net WebAPI containing Models, Services and Controllers folders

Following libraries were used 
1.NetTopologySuite (To find the nearest points)
2.Microsoft.Extensions.Caching.Memory

A sample data (sampledata.csv) is used in the project to get the location of Service trucks. The file is located in "Data" folder in the project. 
The data is loaded into application cache. After each update, data is saved to the cache.

Instructions to run the project.
1.Run the RoadSideAssistance project
2.Swagger UI gets opened.
3.To list all the Service Truck Assistants, invoke /RSA/GetAllAssistants method. This method doesn't have any parameters.
4.To list the 5 nearest Service Truck Assistants to customer location, invoke /RSA/GetNearestServiceTrucks method. This method takes latitude and longitude parameters.
  Following values are some examples to test. Any combination of latitude and longitude values from the sample data file can be used for testing.
  1. latitude= -74.02, longitude=40.12
  2. latitude= -76.00, longitude=43.91
  3. latitude= -74.62, longitude=41.86
  4. latitude= -77.22, longitude=42.96
  5. latitude= -73.54, longitude=40.76

  
5. To reserve a service truck, invoke /RSA/ReserveServiceTruck method. This method takes customername, latitude and longitude parameters.
   Following values are some examples to test.Any combination of latitude and longitude values from the sample data file can be used for testing.
  1. CustomerName= Sri, latitude= -74.02, longitude=40.12
  2. CustomerName= Robert, latitude= -76.00, longitude=43.91
  3. CustomerName= Johnson, latitude= -74.62, longitude=41.86
  4. CustomerName= Amy, latitude= -77.22, longitude=42.96
  5. CustomerName= Mark, latitude= -73.54, longitude=40.76


6. To check if service truck is assigned to a customer, invoke /RSA/GetAllAssignedAssistants method. This method doesn't have any parameters.Review the data to validate.
  
7. To release a service truck, invoke /RSA/ReleaseServiceTruck method. This method takes Customer name and Truck name parameters.
  Provide the customer name and truck name assigned in the step 5
  
8.To check if service truck is released from a customer , invoke /RSA/GetAllAssignedAssistants method. This method doesn't have any parameters. Review the data to validate.
  

  

