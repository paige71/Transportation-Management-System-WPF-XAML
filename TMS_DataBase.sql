use tms; 

drop table if exists carriers;

create table carriers(
	carrierID int not null AUTO_INCREMENT,
    carrier_name varchar(50) not null,    
    carrier_FTL_rate double,
    carrier_LTL_rate double, 
    carrier_reef_charge double,
    primary key (carrierID)    
); 

insert into carriers(carrier_name, carrier_FTL_rate, carrier_LTL_rate, carrier_reef_charge)
values 
('Planet Express',5.21,0.3621, 0.08),
('Schooners', 5.05, 0.3434, 0.07),
('Tillman Transport', 5.11, 0.3012, 0.09),
('We Haul', 5.2, 0, 0.065);


drop table if exists carrierToDeport;
create table carrierToDeport(
	carrierToDeportID int not null AUTO_INCREMENT,
	deport_city varchar(50) not null,
    carrier_FTL_availability int,
	carrier_LTL_availability int,
    carrierID int, 
    primary key (carrierToDeportID),
    foreign key (carrierID) REFERENCES carriers(carrierID)
);

insert into carrierToDeport(deport_city,carrier_FTL_availability,carrier_LTL_availability,carrierID)
values 
('Windsor',50, 640,1),
('Hamilton',50,640,1),
('Oshawa',50,640,1),
('Belleville',50,640,1),
('Ottawa',50,640,1),
('London',18,98,2),
('Toronto',18,98,2),
('Kingston',18,98,2),
('Windsor',24,35,3),
('London',18,45,3),
('Hamilton',18,45,3),
('Ottawa',11,0,4),
('Toronto',11,0,4);

drop table if exists orders;
create table orders(
	orderID int not null AUTO_INCREMENT,
    client_name varchar(50),
    job_type int,
    quantity int,
    origin varchar(50),
    relevant_cities varchar(80),
    destination varchar(50),
    van_type int,
    order_status varchar(50),
    create_time date,
    carrierID int,
    primary key (orderID),
    foreign key (carrierID) REFERENCES carriers(carrierID)
);

drop table if exists trips;
create table trips(
	tripID int not null AUTO_INCREMENT,
    orderID int,
    processing_time int,
    primary key (tripID),
    foreign key (orderID) REFERENCES orders(orderID)
);

drop table if exists clients;
create table clients(
clientID int not null AUTO_INCREMENT,
client_name varchar(50),
client_status varchar(10),
primary key (clientID)
);

drop table if exists invoices;
create table invoices(
	invoiceID int not null auto_increment,
    orderID int,
    invoice_date date,
    amount double,
    distance int,
    surcharge int,
    trip_time double,
    primary key (invoiceID),
    foreign key (orderID) REFERENCES orders(orderID)
);

create table users(
	userID int, 
    userName varchar(50), 
    userPassword varchar(50), 
    userRole varchar(50),
    primary key (userID)
);

insert into users (userID, userName, userPassword, userRole) 
values
(1, 'admin', '123', 'admin'),
(2, 'buyer', '123', 'buyer'),
(3, 'planner', '123', 'planner');

create table route(
	routeID int(2) not null auto_increment, 
    TruckLoadType varchar(3),
    deport_city varchar(10),
    end_city varchar(10), 
    totalDistance int(3), 
    totalTime varchar(6),
    relevantCity1 varchar(10),
	relevantCity2 varchar(10),
	surcharge int(3),
	primary key (routeID)
);

insert into route (TruckLoadType, deport_city, end_city, totalDistance, totalTime, relevantCity1, relevantCity2, surcharge) 
values 
('FTL', 'Windsor','London', '191', '6.5','London', '','0'),
('FTL', 'Windsor','Hamilton', '319', '7.75','London', '','0'),
('FTL', 'Windsor','Toronto', '387', '9.5','London', '','0'),
('FTL', 'Windsor','Oshawa', '447', '10.8','London', '','0'),
('FTL', 'Windsor','Belleville', '581', '12.45','London', '','150'),
('FTL', 'Windsor','Kingston', '663', '13.65','London', '','150'),
('FTL', 'Windsor','Ottawa', '859', '16.15','London', '','150'),
('FTL', 'London','Hamilton', '128', '5.75','Windsor', 'Hamilton','0'),
('FTL', 'London','Toronto', '196', '7','Windsor', 'Hamilton','0'),
('FTL', 'London','Oshawa', '256', '8.3','Windsor', 'Hamilton','0'),
('FTL', 'London','Belleville', '390', '9.95','Windsor', 'Hamilton','0'),
('FTL', 'London','Kingston', '472', '11.15','Windsor', 'Hamilton','0'),
('FTL', 'London','Ottawa', '668', '13.65','Windsor', 'Hamilton','150'),
('FTL', 'Hamilton','Toronto', '68', '5.25','London', 'Toronto','0'),
('FTL', 'Hamilton','Oshawa', '128', '6.55','London', 'Toronto','0'),
('FTL', 'Hamilton','Belleville', '262', '8.2','London', 'Toronto','0'),
('FTL', 'Hamilton','Kingston', '344', '9.4','London', 'Toronto','0'),
('FTL', 'Hamilton','Ottawa', '540', '11.9','London', 'Toronto','0'),
('FTL', 'Toronto','Oshawa', '60', '5.3','Hamilton', 'Oshawa','0'),
('FTL', 'Toronto','Belleville', '194', '6.95','Hamilton', 'Oshawa','0'),
('FTL', 'Toronto','Kingston', '276', '8.15','Hamilton', 'Oshawa','0'),
('FTL', 'Toronto','Ottawa', '472', '10.65','Hamilton', 'Oshawa','0'),
('FTL', 'Oshawa','Belleville', '134', '5.65','Toronto', 'Belleville','0'),
('FTL', 'Oshawa','Kingston', '216', '6.85','Toronto', 'Belleville','0'),
('FTL', 'Oshawa','Ottawa', '412', '9.35','Toronto', 'Belleville','0'),
('FTL', 'Belleville','Kingston', '82', '5.2','Oshawa', 'Kingston','0'),
('FTL', 'Belleville','Ottawa', '278', '7.7','Oshawa', 'Kingston','0'),
('FTL', 'Kingston','Ottawa', '196', '6.5','Belleville', 'Ottawa','0'),
('LTL', 'Windsor','London', '191', '6.5','London', '','0'),
('LTL', 'Windsor','Hamilton', '319', '9.75','London', '','0'),
('LTL', 'Windsor','Toronto', '387', '13.5','London', '','150'),
('LTL', 'Windsor','Oshawa', '447', '16.8','London', '','150'),
('LTL', 'Windsor','Belleville', '581', '20.45','London', '','150'),
('LTL', 'Windsor','Kingston', '663', '23.65','London', '','150'),
('LTL', 'Windsor','Ottawa', '859', '28.15','London', '','300'),
('LTL', 'London','Hamilton', '128', '5.75','Windsor', 'Hamilton','0'),
('LTL', 'London','Toronto', '196', '9','Windsor', 'Hamilton','0'),
('LTL', 'London','Oshawa', '256', '12.3','Windsor', 'Hamilton','150'),
('LTL', 'London','Belleville', '390', '15.95','Windsor', 'Hamilton','150'),
('LTL', 'London','Kingston', '472', '19.15','Windsor', 'Hamilton','150'),
('LTL', 'London','Ottawa', '668', '23.65','Windsor', 'Hamilton','150'),
('LTL', 'Hamilton','Toronto', '68', '5.25','London', 'Toronto','0'),
('LTL', 'Hamilton','Oshawa', '128', '8.55','London', 'Toronto','0'),
('LTL', 'Hamilton','Belleville', '262', '12.2','London', 'Toronto','150'),
('LTL', 'Hamilton','Kingston', '344', '15.4','London', 'Toronto','150'),
('LTL', 'Hamilton','Ottawa', '540', '19.9','London', 'Toronto','150'),
('LTL', 'Toronto','Oshawa', '60', '5.3','Hamilton', 'Oshawa','0'),
('LTL', 'Toronto','Belleville', '194', '8.95','Hamilton', 'Oshawa','0'),
('LTL', 'Toronto','Kingston', '276', '12.15','Hamilton', 'Oshawa','150'),
('LTL', 'Toronto','Ottawa', '472', '16.65','Hamilton', 'Oshawa','150'),
('LTL', 'Oshawa','Belleville', '134', '5.65','Toronto', 'Belleville','0'),
('LTL', 'Oshawa','Kingston', '216', '8.85','Toronto', 'Belleville','0'),
('LTL', 'Oshawa','Ottawa', '412', '13.35','Toronto', 'Belleville','150'),
('LTL', 'Belleville','Kingston', '82', '5.2','Oshawa', 'Kingston','0'),
('LTL', 'Belleville','Ottawa', '278', '9.7','Oshawa', 'Kingston','0'),
('LTL', 'Kingston','Ottawa', '196', '6.5','Belleville', 'Ottawa','0');

