-- phpMyAdmin SQL Dump
-- version 4.5.4.1deb2ubuntu2
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Erstellungszeit: 21. Mrz 2017 um 18:43
-- Server-Version: 5.7.17-0ubuntu0.16.04.1
-- PHP-Version: 7.0.13-0ubuntu0.16.04.1

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Datenbank: `fuldaflats`
--

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Favorite`
--

CREATE TABLE `Favorite` (
  `userId` int(11) DEFAULT NULL,
  `offerId` int(11) DEFAULT NULL,
  `id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Daten für Tabelle `Favorite`
--

INSERT INTO `Favorite` (`userId`, `offerId`, `id`) VALUES
(1, 2, 1),
(2, 1, 2),
(2, 3, 3),
(4, 3, 4),
(9, 5, 5),
(9, 4, 6),
(9, 7, 7),
(9, 9, 8),
(9, 8, 9),
(8, 11, 10),
(2, 8, 11);

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `MediaObject`
--

CREATE TABLE `MediaObject` (
  `type` int(11) DEFAULT NULL,
  `mainUrl` varchar(255) DEFAULT NULL,
  `thumbnailUrl` varchar(255) DEFAULT NULL,
  `creationDate` datetime DEFAULT NULL,
  `userId` int(11) DEFAULT NULL,
  `offerId` int(11) DEFAULT NULL,
  `createdByUserId` int(11) DEFAULT NULL,
  `id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Daten für Tabelle `MediaObject`
--

INSERT INTO `MediaObject` (`type`, `mainUrl`, `thumbnailUrl`, `creationDate`, `userId`, `offerId`, `createdByUserId`, `id`) VALUES
(1, '/uploads/file_1481746739433_apart4.jpg', '/uploads/file_1481746739433_apart4.jpg', '2016-12-14 19:40:34', NULL, 1, 1, 1),
(1, '/uploads/file_1481746739498_apart4-2.jpg', '/uploads/file_1481746739498_apart4-2.jpg', '2016-12-14 19:40:34', NULL, 1, 1, 2),
(1, '/uploads/file_1481746739357_apart3-1.jpg', '/uploads/file_1481746739357_apart3-1.jpg', '2016-12-14 19:40:34', NULL, 1, 1, 3),
(1, '/uploads/file_1481746739379_apart3-2.jpg', '/uploads/file_1481746739379_apart3-2.jpg', '2016-12-14 19:40:34', NULL, 1, 1, 4),
(1, '/uploads/file_1481747585895_sampleA.jpg', '/uploads/file_1481747585895_sampleA.jpg', '2016-12-14 19:40:34', NULL, 2, 2, 5),
(1, '/uploads/file_1481756977049_apart3-1.jpg', '/uploads/file_1481756977049_apart3-1.jpg', '2016-12-14 19:40:34', NULL, 3, 1, 6),
(1, '/uploads/file_1481756977098_apart5.jpg', '/uploads/file_1481756977098_apart5.jpg', '2016-12-14 19:40:34', NULL, 3, 1, 7),
(1, '/uploads/file_1481756977084_apart4-2.jpg', '/uploads/file_1481756977084_apart4-2.jpg', '2016-12-14 19:40:34', NULL, 3, 1, 8),
(1, '/uploads/file_1481757434295_apart6.jpg', '/uploads/file_1481757434295_apart6.jpg', '2016-12-14 19:40:34', NULL, 4, 1, 10),
(1, '/uploads/file_1481757433948_apart4-2.jpg', '/uploads/file_1481757433948_apart4-2.jpg', '2016-12-14 19:40:34', NULL, 4, 1, 11),
(1, '/uploads/file_1481757433886_apart3-2.jpg', '/uploads/file_1481757433886_apart3-2.jpg', '2016-12-14 19:40:34', NULL, 4, 1, 12),
(1, '/uploads/file_1481758355991_sampleB.jpg', '/uploads/file_1481758355991_sampleB.jpg', '2016-12-14 19:40:34', NULL, 5, 2, 13),
(1, '/uploads/file_1481758355969_apart11.jpg', '/uploads/file_1481758355969_apart11.jpg', '2016-12-14 19:40:34', NULL, 5, 2, 14),
(1, '/uploads/file_1481789075763_bedroom-389254_1280.jpg', '/uploads/file_1481789075763_bedroom-389254_1280.jpg', '2016-12-14 19:40:34', NULL, 7, 4, 15),
(1, '/uploads/file_1481789077044_computer-room-1488311_1280.jpg', '/uploads/file_1481789077044_computer-room-1488311_1280.jpg', '2016-12-14 19:40:34', NULL, 7, 4, 16),
(1, '/uploads/file_1481789078455_interior-1026446_1280.jpg', '/uploads/file_1481789078455_interior-1026446_1280.jpg', '2016-12-14 19:40:34', NULL, 7, 4, 17),
(1, '/uploads/file_1481789079721_interior-1026454_1280.jpg', '/uploads/file_1481789079721_interior-1026454_1280.jpg', '2016-12-14 19:40:34', NULL, 7, 4, 18),
(1, '/uploads/file_1481789664710_design-interior-plant-lamps-73382.jpeg', '/uploads/file_1481789664710_design-interior-plant-lamps-73382.jpeg', '2016-12-14 19:40:34', NULL, 9, 3, 19),
(1, '/uploads/file_1481789664711_pexels-photo-245219.jpeg', '/uploads/file_1481789664711_pexels-photo-245219.jpeg', '2016-12-14 19:40:34', NULL, 9, 3, 20),
(1, '/uploads/file_1481789664635_pexels-photo-105934.jpeg', '/uploads/file_1481789664635_pexels-photo-105934.jpeg', '2016-12-14 19:40:34', NULL, 9, 3, 21),
(1, '/uploads/file_1481790593235_pexels-photo-164595.jpeg', '/uploads/file_1481790593235_pexels-photo-164595.jpeg', '2016-12-15 08:16:36', NULL, 11, 8, 22),
(1, '/uploads/file_1481790681499_pexels-photo-94865.jpeg', '/uploads/file_1481790681499_pexels-photo-94865.jpeg', '2016-12-15 08:16:36', NULL, 11, 8, 23),
(1, '/uploads/file_1481790725401_pexels-photo-189333.jpeg', '/uploads/file_1481790725401_pexels-photo-189333.jpeg', '2016-12-15 08:16:36', NULL, 11, 8, 24),
(1, '/uploads/file_1481790753729_pexels-photo-245032.jpeg', '/uploads/file_1481790753729_pexels-photo-245032.jpeg', '2016-12-15 08:16:36', NULL, 11, 8, 25),
(1, '/uploads/file_1481805441676_Arbeitszimmer.jpg', '/uploads/file_1481805441676_Arbeitszimmer.jpg', '2016-12-15 08:16:36', NULL, 16, 3, 26),
(1, '/uploads/file_1481805441929_Bad.jpg', '/uploads/file_1481805441929_Bad.jpg', '2016-12-15 08:16:36', NULL, 16, 3, 27),
(1, '/uploads/file_1481805441920_Aufzug.jpg', '/uploads/file_1481805441920_Aufzug.jpg', '2016-12-15 08:16:36', NULL, 16, 3, 28);

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Offer`
--

CREATE TABLE `Offer` (
  `title` varchar(255) DEFAULT NULL,
  `offerType` varchar(12) DEFAULT NULL,
  `description` text,
  `rent` int(11) DEFAULT NULL,
  `rentType` varchar(4) DEFAULT NULL,
  `rooms` int(11) DEFAULT NULL,
  `sideCosts` int(11) DEFAULT NULL,
  `fullPrice` double DEFAULT NULL,
  `deposit` double DEFAULT NULL,
  `commission` double DEFAULT NULL,
  `priceType` varchar(8) DEFAULT NULL,
  `street` varchar(255) DEFAULT NULL,
  `zipCode` varchar(5) DEFAULT NULL,
  `houseNumber` int(11) DEFAULT NULL,
  `city` varchar(255) DEFAULT NULL,
  `floor` int(2) DEFAULT NULL,
  `size` double DEFAULT NULL,
  `furnished` tinyint(1) DEFAULT NULL,
  `pets` tinyint(1) DEFAULT NULL,
  `bathroomNumber` int(11) DEFAULT NULL,
  `bathroomDescription` varchar(255) DEFAULT NULL,
  `kitchenDescription` varchar(255) DEFAULT NULL,
  `cellar` tinyint(1) DEFAULT NULL,
  `parking` tinyint(1) DEFAULT NULL,
  `elevator` tinyint(1) DEFAULT NULL,
  `accessability` tinyint(1) DEFAULT NULL,
  `wlan` tinyint(1) DEFAULT NULL,
  `lan` tinyint(1) DEFAULT NULL,
  `internetSpeed` int(11) DEFAULT NULL,
  `heatingDescription` varchar(255) DEFAULT NULL,
  `television` varchar(255) DEFAULT NULL,
  `dryer` tinyint(1) DEFAULT NULL,
  `washingMachine` tinyint(1) DEFAULT NULL,
  `telephone` tinyint(1) DEFAULT NULL,
  `status` int(11) DEFAULT NULL,
  `creationDate` datetime DEFAULT NULL,
  `lastModified` datetime DEFAULT NULL,
  `longitude` double DEFAULT NULL,
  `latitude` double DEFAULT NULL,
  `uniDistance` double DEFAULT NULL,
  `landlord` int(11) DEFAULT NULL,
  `id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Daten für Tabelle `Offer`
--

INSERT INTO `Offer` (`title`, `offerType`, `description`, `rent`, `rentType`, `rooms`, `sideCosts`, `fullPrice`, `deposit`, `commission`, `priceType`, `street`, `zipCode`, `houseNumber`, `city`, `floor`, `size`, `furnished`, `pets`, `bathroomNumber`, `bathroomDescription`, `kitchenDescription`, `cellar`, `parking`, `elevator`, `accessability`, `wlan`, `lan`, `internetSpeed`, `heatingDescription`, `television`, `dryer`, `washingMachine`, `telephone`, `status`, `creationDate`, `lastModified`, `longitude`, `latitude`, `uniDistance`, `landlord`, `id`) VALUES
('Cozy Flat in Fulda', 'FLAT', 'The room is large, bright, comfy and fun. There is a 40"  tv, play station, mini candy grabber machine and a 93" plush bear that doubles up as a cozy sofa. We are at the very heart of Fulda. Excellent transport links to anywhere in Fulda.', 290, 'WARM', 2, 200, 490, 580, 125, 'MONTH', 'Professor Heller Strasse', '36043', 12, 'Fulda', 4, 72, NULL, 1, 1, 'Shower & Tub & WC', 'Fridge & Stove', 1, 1, NULL, NULL, 1, 1, 100000, 'Electricity', 'DSL', NULL, 1, 1, 1, '2016-12-11 20:13:50', '2016-12-14 20:13:50', 9.6779811, 50.513716, 5.7, 1, 1),
('The one and only couch', 'COUCH', 'Best couch in the world! Come and nap on my couch for a really cheap price! Have a party in Fulda, check in and check out very flexible.', 10, NULL, NULL, NULL, 10, 10, NULL, NULL, 'Frankfurter Straße', '36043', 23, 'Fulda', 3, 2, 1, 1, 1, 'Shower & WC', 'Fridge & Stove & Oven', 1, 1, 1, NULL, 1, 1, 50000, 'Gas', 'Cable', NULL, NULL, NULL, 1, '2016-12-10 20:28:47', '2016-12-14 20:28:47', 9.6783443, 50.5280137, 4.12, 2, 2),
('Entire Apartment in Fulda', 'FLAT', 'Comfortable studio in Fulda, Westminster area. A stone throw away from Fulda university. Very close to Fulda transportation and all amenities. Nice and clean in a beautiful period building.', 240, 'COLD', 2, 150, 390, 480, 100, 'MONTH', 'Heinrichstraße', '36037', 13, 'Fulda', 1, 32, 1, 1, 1, 'Tub & WC', 'Fridge & Oven', 1, 1, NULL, NULL, NULL, NULL, 150000, 'Gas', 'SAT', NULL, 1, 1, 1, '2016-12-14 23:04:24', '2016-12-14 23:04:24', 9.6817287, 50.5540386, 1.21, 1, 3),
('Large, Bright, Private Double Room', 'FLAT', 'A relaxed home in the heart of this famous colourful locality. 5 minutes from trains and buses, 24 hour transport. A quiet street in a vibrant, unique area of Fulda.', 440, 'WARM', 4, 500, 940, 880, 350, 'MONTH', 'Bahnhofstraße', '36037', 78, 'Fulda', 3, 232, NULL, 1, 2, 'Shower & Tub & WC', 'Fridge & Stove & Oven', 1, 1, 1, 1, 1, 1, 200000, 'Electricity', 'Cable', 1, 1, 1, 1, '2016-12-13 23:12:06', '2016-12-14 23:12:06', 9.6819846, 50.5536876, 1.25, 1, 4),
('Nice room for sublet renting near the university of Fulda', 'SUBLET', '1 room apartment close to the university of Fulda. Available for sublet renting from 12/03/2017 to 03/09/2017.', 800, 'WARM', 1, 200, 1000, 400, 0, 'SEMESTER', 'Leipziger Straße', '36037', 129, 'Fulda', 3, 23, 1, NULL, 1, 'Shower & WC', 'Fridge & Stove & Oven', 1, NULL, 1, NULL, 1, NULL, 50000, 'Oil', 'Cable', NULL, 1, NULL, 1, '2016-12-10 23:25:24', '2016-12-14 23:25:24', 9.6793078, 50.5593434, 0.7, 2, 5),
('Couch for you', 'COUCH', 'I have been in Germany since April and I\'m studying Public Health in the Hochschule Fulda.\n\nI got a job in Amazon Bad Hersfeld till the end of the year. Ich work almost every day until midnight. At this time I can\'t find any train to Fulda. I therefore have to wait until 3.25 am for the train. The train station is cold. I therefore need a warm place, where i can wait for these 3 hours. I don\'t have to sleep. I can read in this time.', 100, 'COLD', NULL, NULL, 230, 300, NULL, 'DAY', 'Petersberger Str. 36', '36037', 36, 'Fulda', 2, 29, NULL, 1, 2, 'Shower & Tub & WC', 'Fridge & Stove & Oven', 1, 1, 1, 1, 1, 1, 50, 'Electricity', 'SAT', 1, 1, 1, 1, '2016-12-15 07:50:44', '2016-12-15 07:50:44', 9.6903449, 50.551201, 1.58, 4, 7),
('Sleepover for Halle 8 Party next weekend (17.12.16)', 'PARTY', 'Hey folks! I offer my guestroom for next weekend\'s Halle 8 Party at Fulda University. Just bring a crate of beer for preloading at my place, no other fees required! If you study at Fulda University and if you are not from Fulda, this offer is the ideal deal for you, just contact me.', 0, 'COLD', NULL, NULL, 0, 0, NULL, 'DAY', 'am See', '36100', 21, 'Petersberg', 1, 5, 1, NULL, 1, 'Shower & Tub & WC', 'Fridge & Stove & Oven', NULL, 1, 1, NULL, 1, NULL, 200000, 'Electricity', 'Cable', NULL, NULL, NULL, 1, '2016-12-15 07:52:35', '2016-12-15 07:52:35', 9.7161928, 50.5559024, 2.47, 1, 8),
('Offer a nice little room in my shared apartement', 'SHARE', 'My room mate finished his bachelor therefore he leaves us in Petersberg. So there is a new room availabe in our shared apartment.\nWe are three other students at the university of fulda.\nMyself is a student of oecotrophology.\nThe apartment includes all standard desires:\n - internet\n- fridge\n- stove\n- oven\n- shower\n\nIf you are interested then don\'t hesitate and contact me.', 200, 'WARM', 1, 50, 250, 400, 0, 'MONTH', 'Brauhausstraße', '36100', 10, 'Petersberg', 2, 22, NULL, NULL, 1, 'Shower & WC', 'Fridge & Stove & Oven', NULL, NULL, NULL, NULL, 1, 1, 50, 'Oil', 'Cable', NULL, NULL, 1, 1, '2016-12-15 07:53:49', '2016-12-15 07:53:49', 9.7120773, 50.5604219, 2.03, 3, 9),
('Big Flat at Fulda suburbia', 'FLAT', 'This is a really nice flat at Fulda Aschenberg, a suburban area of Fulda. It is suitable for two persons. There is a Bus Station 100 meters from the flat, which stops direct at the HS Fulda.', 600, 'WARM', 5, 150, 750, 1200, 50, 'MONTH', 'Stresemannstrasse', '36039', 12, 'Fulda', 1, 120, 1, NULL, 1, 'Shower & Tub & WC', 'Fridge & Stove & Oven', 1, NULL, NULL, 1, 1, 1, 100, 'Gas', 'Cable', NULL, 1, NULL, 1, '2016-12-13 08:19:14', '2016-12-15 08:19:14', 9.6496722, 50.5758143, 2.74, 8, 11),
('Nice little flat in Fulda', 'FLAT', 'Nice little appartment in Fulda.', 300, 'WARM', 1, 80, 380, 850, 0, 'MONTH', 'Wiesenmühlenstraße', '36037', 15, 'Fulda', 2, 30, NULL, 1, 1, 'Shower & WC', 'Fridge & Stove', 1, 1, NULL, NULL, NULL, NULL, 200000, 'Oil', 'Cable', NULL, NULL, NULL, 1, '2016-12-15 12:33:23', '2016-12-15 12:33:23', 9.6706991, 50.5525802, 1.67, 3, 16);

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Review`
--

CREATE TABLE `Review` (
  `title` varchar(255) DEFAULT NULL,
  `rating` int(11) DEFAULT NULL,
  `comment` text,
  `creationDate` datetime DEFAULT NULL,
  `offerId` int(11) DEFAULT NULL,
  `userId` int(11) DEFAULT NULL,
  `id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Daten für Tabelle `Review`
--

INSERT INTO `Review` (`title`, `rating`, `comment`, `creationDate`, `offerId`, `userId`, `id`) VALUES
('A real avenger\'s couch', 4, 'I had a real blast with Steve, hitting the streets of Fulda last Saturday Night! His appartment is clean and this couch is quite confortable. The only downside is his quite old-fashioned humor...', '2016-12-14 23:21:18', 2, 1, 1),
('Really nice!', 5, 'Tony is a terrific guy and I had a blast partying with him last weekend. I can definitly recommend his guestroom, the location and neighbourhood is also really nice.', '2016-12-15 10:45:33', 8, 2, 2),
('Old but gold!', 5, 'I stayed at steve\'s for a business trip. He has a great sense of humor and told me to go to a really nice restaurant. I can definitly recommend his couch!', '2016-12-15 10:48:16', 2, 6, 3),
('Really bad', 1, 'Dirty ugly flat. Don\'t go there!', '2016-12-15 12:32:15', 8, 3, 4);

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `sessions`
--

CREATE TABLE `sessions` (
  `session_id` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `expires` int(11) UNSIGNED NOT NULL,
  `data` text CHARACTER SET utf8mb4 COLLATE utf8mb4_bin
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Daten für Tabelle `sessions`
--

INSERT INTO `sessions` (`session_id`, `expires`, `data`) VALUES
('04akaFuqbWt55uTMQj1tghfD8xYElbIE', 1485573895, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('5765lq4Mxkctp3ofgj2f8hh_ia040t92', 1485608579, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('6QYfO9W8myKUdQCEN-Sd-HVFVVmi0D8m', 1485570218, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('B5Tk1nwkhKfcpBW2c58onSd92Pa1XUAB', 1485573916, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('BCuY-imwbXIYOHPXCw5Roio5BYxLuu7L', 1485573917, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('EXuQy8CcE8GrUAA90mxiYImz-61lgSyN', 1485573896, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('Gvsa_-Utc3OYPXhFG9M42w5To0VRg1kg', 1485630141, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('LAKhrl3UFUf6nDbG0Bp7G8LjiBspRqob', 1485573906, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('NDD5C-J2z2WLi9Z4AwQBPn1Y6WQbXW5a', 1485557383, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('Nma4GZqWSBfjf6xWAjgJanTeBxrTO0cQ', 1485636270, '{"cookie":{"originalMaxAge":false,"expires":false,"httpOnly":true,"path":"/"},"auth":true,"search":{"uniDistance":{},"rent":{},"size":{},"id":{"in":["3","3","7","1","1","4","4","11","11"]},"status":1},"user":{"id":11,"email":"test@test.de","type":1,"firstName":"Test","lastName":"Test 2","phoneNumber":null,"gender":"female","officeAddress":null,"averageRating":0,"profilePicture":"/uploads/cupcake.png"}}'),
('NnTiI5vLzmtIsyiRN3163B0lIf-9-LRf', 1485592377, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('Nw9YsehAJ0EeMpPnM0iVDKrisrW53A20', 1485573895, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('PErViK0O3ahh-VCRimkqvIHIIKvQrWfx', 1485573896, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('SuFJyuzzG5Sxq0GUnslFv9cT4wGJ5FFe', 1485573917, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('VGzwjHMj4VURHEN5T3a_M47UAzEGh_jP', 1485556818, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"},"search":{"offerType":null,"uniDistance":{},"rent":{},"size":{},"tags":["english","german","french","spanish","italian","portuguese","turkish","russian","ukrainian","persian","arabic","japanese","chinese"]}}'),
('XcneBnkuyD9Yc93I2IvANKDVadhh7gP5', 1485573903, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('YA0NFqO3jLSLSiy_L2r_dFNjz69co2DG', 1485630167, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"},"search":{"offerType":null,"uniDistance":{},"rent":{},"size":{},"tags":["english","german","french","spanish","italian","portuguese","turkish","russian","ukrainian","persian","arabic","japanese","chinese"]}}'),
('YcY-ZywVscr8ktFTK_7_eZu4GVQHQSCh', 1485573903, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('_dh7GS57h_fH8cXkcUhMwjxV9oRnr_0b', 1485573903, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('aRjXcMr5EG93JvOQ-9KkeF7SwPvX6avN', 1485573919, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('dEq4QXBhmnS3yQ5dsuXL2qeB5jZNKcW5', 1485563303, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('e2FV8sAoXfQCJ7mkPgkjiVoqJxtlZGzz', 1485573906, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('kBEmTDZNfGLdkS53h4dV5V5kDyuEQ5kx', 1485624641, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('kLTLNtkKYyRhhEcfn83gNKiOOREIChBy', 1485570232, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('snLCFuXoQlLwrEobtdpQiMHJ3rf5uW5u', 1485570214, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('vJAr6BpiLGEwrSNnneKC1N9wOqSIHv8n', 1485573916, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('xJkc7znsNz8w860okxYUGfx4Hhuppi2C', 1485570244, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('yHo-I7iM_7an1EhffkBK4j8aXRETwu6A', 1485624625, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('yaula1s-DmfIDYwZypF4lO9ENXTQLIuF', 1485573906, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}'),
('yz8YKlbMw2W1KUuTqjPL7N3xhc_gbbxE', 1485573906, '{"cookie":{"originalMaxAge":null,"expires":null,"httpOnly":true,"path":"/"}}');

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Tag`
--

CREATE TABLE `Tag` (
  `title` varchar(255) DEFAULT NULL,
  `offerId` int(11) DEFAULT NULL,
  `id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Daten für Tabelle `Tag`
--

INSERT INTO `Tag` (`title`, `offerId`, `id`) VALUES
('party', 2, 39),
('beerpong', 2, 40),
('drone racing', 2, 41),
('bodybuilding', 2, 42),
('esports', 2, 43),
('electrical engineering', 5, 80),
('accounting, finance, controlling', 5, 81),
('public health', 5, 82),
('english', 3, 117),
('german', 3, 118),
('french', 7, 125),
('oecotrophology', 9, 130),
('english', 1, 131),
('german', 1, 132),
('english', 4, 135),
('german', 4, 136),
('computer science', 8, 137),
('electrical engineering', 8, 138),
('party', 8, 139),
('drone racing', 8, 140),
('english', 11, 165),
('german', 11, 166),
('computer science', 11, 167),
('music', 11, 168),
('computer games', 11, 169),
('party', 11, 170),
('food processing', 16, 183),
('health management', 16, 184);

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `User`
--

CREATE TABLE `User` (
  `email` varchar(255) DEFAULT NULL,
  `password` varchar(255) DEFAULT NULL,
  `type` int(1) DEFAULT NULL,
  `firstName` varchar(255) DEFAULT NULL,
  `lastName` varchar(255) DEFAULT NULL,
  `birthday` datetime DEFAULT NULL,
  `upgradeDate` datetime DEFAULT NULL,
  `creationDate` datetime DEFAULT NULL,
  `phoneNumber` varchar(255) DEFAULT NULL,
  `zipCode` varchar(5) DEFAULT NULL,
  `city` varchar(255) DEFAULT NULL,
  `street` varchar(255) DEFAULT NULL,
  `houseNumber` varchar(5) DEFAULT NULL,
  `gender` varchar(6) DEFAULT NULL,
  `officeAddress` varchar(4000) DEFAULT NULL,
  `averageRating` double DEFAULT NULL,
  `loginAttempts` int(10) DEFAULT NULL,
  `isLocked` tinyint(1) DEFAULT NULL,
  `profilePicture` varchar(255) NOT NULL DEFAULT '/uploads/cupcake.png',
  `id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Daten für Tabelle `User`
--

INSERT INTO `User` (`email`, `password`, `type`, `firstName`, `lastName`, `birthday`, `upgradeDate`, `creationDate`, `phoneNumber`, `zipCode`, `city`, `street`, `houseNumber`, `gender`, `officeAddress`, `averageRating`, `loginAttempts`, `isLocked`, `profilePicture`, `id`) VALUES
('tony.stark@fuldaflats.de', 'NIE2X9yIRe08OVVnP2hb+5/I2nK7cm3AjJUFEO7qmPkznAX70UWASC3k2VuI67qB2zfCfAQCsf8P+OhBoa6ChA==', 2, 'Tony', 'Stark', '1965-04-03 22:00:00', '2016-12-14 19:58:30', '1970-01-01 00:00:00', '0661152512', '36119', 'Neuhof', 'Berliner Straße', '30', 'male', '36119 Neuhof, Berliner Straße 30', 3, 0, NULL, '/uploads/file_1481745410084_ironman.jpg', 1),
('steve.rodgers@fuldaflats.de', 'NIE2X9yIRe08OVVnP2hb+5/I2nK7cm3AjJUFEO7qmPkznAX70UWASC3k2VuI67qB2zfCfAQCsf8P+OhBoa6ChA==', 2, 'Steve', 'Rodgers', '1915-06-29 22:00:00', '2016-12-14 20:01:19', '1970-01-01 00:00:00', '0661152912', '36037', 'Fulda', 'Leipziger Straße', '10', 'male', '36037 Fulda, Leipziger Straße 10', 4.5, NULL, NULL, '/uploads/file_1481757842064_captain-america-861757_1920.jpg', 2),
('mariane.mueller@web.de', 'NIE2X9yIRe08OVVnP2hb+5/I2nK7cm3AjJUFEO7qmPkznAX70UWASC3k2VuI67qB2zfCfAQCsf8P+OhBoa6ChA==', 2, 'Mariane', 'Müller', '1975-10-09 22:00:00', '2016-12-14 20:02:53', '1970-01-01 00:00:00', '0661052912', '36039', 'Fulda', 'Frankfurter Straße', '89', 'female', '36039 Fulda, Frankfurter Straße 89', 1, NULL, NULL, '/uploads/file_1481745877934_user5.jpg', 3),
('louisa1991@gmx.de', 'NIE2X9yIRe08OVVnP2hb+5/I2nK7cm3AjJUFEO7qmPkznAX70UWASC3k2VuI67qB2zfCfAQCsf8P+OhBoa6ChA==', 2, 'Louisa', 'Schmidt', '1991-06-19 22:00:00', '2016-12-14 20:06:37', '1970-01-01 00:00:00', '0661100812', '36039', 'Fulda', 'Henrich Straße', '30', 'female', '36039 Fulda, Henrich Straße 30', 1, NULL, NULL, '/uploads/file_1481789206615_eye-716008_640.jpg', 4),
('max.mustermann@fakemail.com', 'NIE2X9yIRe08OVVnP2hb+5/I2nK7cm3AjJUFEO7qmPkznAX70UWASC3k2VuI67qB2zfCfAQCsf8P+OhBoa6ChA==', 1, 'Max', 'Mustermann', '1983-01-31 23:00:00', '1970-01-01 00:00:00', '1970-01-01 00:00:00', NULL, NULL, NULL, NULL, NULL, 'male', NULL, 0, NULL, NULL, '/img/cupcake.png', 5),
('horstmueller@city-immobilien.com', 'NIE2X9yIRe08OVVnP2hb+5/I2nK7cm3AjJUFEO7qmPkznAX70UWASC3k2VuI67qB2zfCfAQCsf8P+OhBoa6ChA==', 1, 'Horst', 'Müller', '1956-09-04 22:00:00', '1970-01-01 00:00:00', '1970-01-01 00:00:00', NULL, NULL, NULL, NULL, NULL, 'male', NULL, 0, NULL, NULL, '/uploads/file_1481746233354_user4.jpg', 6),
('christinafranke@frankeimmobilien.com', 'NIE2X9yIRe08OVVnP2hb+5/I2nK7cm3AjJUFEO7qmPkznAX70UWASC3k2VuI67qB2zfCfAQCsf8P+OhBoa6ChA==', 1, 'Christina', 'Franke', '1980-08-20 22:00:00', '1970-01-01 00:00:00', '1970-01-01 00:00:00', NULL, NULL, NULL, NULL, NULL, 'female', NULL, 0, NULL, NULL, '/img/cat.png', 7),
('marie.herbener@web.de', 'wzOLuAZC1bcR24aCykOTvaDJhz3VXxzd+rEZ7cMJXHtVJLqHRlPvtzzBDm9ac+4GIavaNj/UyyX2hTBttBbcmg==', 2, 'Marie', 'Herbener', '1984-05-12 22:00:00', '2016-12-15 08:18:23', '1970-01-01 00:00:00', '01777430854', '34286', 'Spangenberg', 'An der Körsche', '3', 'female', '34286 Spangenberg, An der Körsche 3', 1, NULL, NULL, '/uploads/file_1481789820439_man-1895084_1920.jpg', 8),
('phasenauer@fuldaflats.de', 'NIE2X9yIRe08OVVnP2hb+5/I2nK7cm3AjJUFEO7qmPkznAX70UWASC3k2VuI67qB2zfCfAQCsf8P+OhBoa6ChA==', 1, 'Patrick', 'Hasenauer', '1992-03-02 23:00:00', '1970-01-01 00:00:00', '1970-01-01 00:00:00', NULL, NULL, NULL, NULL, NULL, 'male', NULL, 0, 1, NULL, '/uploads/cupcake.png', 9),
('robin42@gmx.de', 'NIE2X9yIRe08OVVnP2hb+5/I2nK7cm3AjJUFEO7qmPkznAX70UWASC3k2VuI67qB2zfCfAQCsf8P+OhBoa6ChA==', 2, 'Robin', 'Meier', '1987-06-27 22:00:00', '2016-12-15 09:19:26', '1970-01-01 00:00:00', '01746866926', '36041', 'Fulda', 'Dieselstraße', '8', 'male', '36041 Fulda, Dieselstraße 8', 1, NULL, NULL, '/uploads/cupcake.png', 10);

--
-- Indizes der exportierten Tabellen
--

--
-- Indizes für die Tabelle `Favorite`
--
ALTER TABLE `Favorite`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `id` (`id`);

--
-- Indizes für die Tabelle `MediaObject`
--
ALTER TABLE `MediaObject`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `id` (`id`);

--
-- Indizes für die Tabelle `Offer`
--
ALTER TABLE `Offer`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `id` (`id`);

--
-- Indizes für die Tabelle `Review`
--
ALTER TABLE `Review`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `id` (`id`);

--
-- Indizes für die Tabelle `sessions`
--
ALTER TABLE `sessions`
  ADD PRIMARY KEY (`session_id`);

--
-- Indizes für die Tabelle `Tag`
--
ALTER TABLE `Tag`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `id` (`id`);

--
-- Indizes für die Tabelle `User`
--
ALTER TABLE `User`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `id` (`id`),
  ADD KEY `email` (`email`);

--
-- AUTO_INCREMENT für exportierte Tabellen
--

--
-- AUTO_INCREMENT für Tabelle `Favorite`
--
ALTER TABLE `Favorite`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;
--
-- AUTO_INCREMENT für Tabelle `MediaObject`
--
ALTER TABLE `MediaObject`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=29;
--
-- AUTO_INCREMENT für Tabelle `Offer`
--
ALTER TABLE `Offer`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;
--
-- AUTO_INCREMENT für Tabelle `Review`
--
ALTER TABLE `Review`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;
--
-- AUTO_INCREMENT für Tabelle `Tag`
--
ALTER TABLE `Tag`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=185;
--
-- AUTO_INCREMENT für Tabelle `User`
--
ALTER TABLE `User`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
