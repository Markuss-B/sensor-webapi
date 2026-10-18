# Sensor Web API

An ASP.NET Core Web API that provides sensor data, sensor management, notification management, and real-time measurement streaming through SignalR.

This service is part of the **Sensor Monitoring System**, developed for Aranet environmental sensors at the University of Latvia.

**Technologies:** C#, .NET 8, ASP.NET Core, MongoDB, SignalR, Docker, Swagger/OpenAPI

## Features

- **Sensor management:** Retrieve registered sensors, inspect their metadata and latest measurements, and update their properties.
- **Historical measurements:** Retrieve a sensor's measurements for the current day.
- **Real-time monitoring:** Stream new measurements to connected clients using SignalR.
- **Notification management:** Retrieve notifications and create, view, update, or delete notification rules.
- **Event-driven updates:** Uses MongoDB Change Streams to detect new sensor measurements.
- **Subscription management:** Tracks connected clients and monitors sensors only while they have subscribers.
- **Change history:** Records updates to sensor properties, including location and activation status.
- **Containerization:** Supports Docker-based deployment and configuration through environment variables.

## Architecture

The Web API acts as the backend for the Angular monitoring dashboard.

It provides REST endpoints for retrieving and modifying data, alongside a SignalR hub for real-time measurement updates.

### Main Components

| Component | Responsibility |
|---|---|
| `SensorController` | Exposes endpoints for retrieving sensors, measurements, and updating sensor properties |
| `NotificationsController` | Exposes endpoints for notifications and notification rule management |
| `SensorService` | Handles sensor queries, updates, and measurement retrieval |
| `NotificationsService` | Handles notification retrieval and rule management |
| `SensorHub` | Manages SignalR connections and sensor subscriptions |
| `SensorWatcherService` | Watches MongoDB for new measurements from subscribed sensors |
| `GroupTrackingService` | Tracks clients subscribed to individual sensors |
| `MongoDb` | Provides MongoDB database access |

<img width="1266" height="603" alt="image" src="https://github.com/user-attachments/assets/0dfbfe14-7703-4538-9a95-0fc2f6540a52" />

## REST API

### Sensor Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/sensor` | Retrieve all registered sensors |
| GET | `/api/sensor/{sensorId}` | Retrieve a sensor by its ID |
| GET | `/api/sensor/{sensorId}/measurements/today` | Retrieve today's measurements for a sensor |
| PATCH | `/api/sensor` | Update sensor properties |

#### Sensor Data

The sensor endpoints return information including:

- Sensor ID
- Location and description
- Active status
- Sensor metadata
- Latest measurement values and timestamp
- Associated MQTT topics

The measurement endpoint returns time-stamped measurement records for the selected sensor.

#### Updating a Sensor

The PATCH endpoint allows updating a sensor's location, description, and activation status.

The API checks whether the sensor exists and whether the submitted properties differ from their current values.

Changes are persisted in MongoDB, with a history of modified properties stored in the `sensorMetadatas` collection.

Setting `isActive` to `false` allows administrators to disable data collection for a sensor. The Sensor Consumer observes these changes and updates its inactive sensor cache accordingly.

### Notification Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/notifications` | Retrieve generated notifications |
| GET | `/api/notifications/rules` | Retrieve all notification rules |
| GET | `/api/notifications/rules/{ruleId}` | Retrieve a rule and its associated notifications |
| POST | `/api/notifications/rules/new` | Create a notification rule |
| PUT | `/api/notifications/rules` | Update an existing rule |
| DELETE | `/api/notifications/rules/{ruleId}` | Delete a notification rule |

#### Notification Rules

Notification rules define conditions under which notifications are generated.

Each rule contains:

- **Name:** Descriptive name of the rule.
- **Sensor ID:** Optional identifier to target a particular sensor.
- **Measurement:** The measurement type being monitored.
- **Operator:** Comparison operator (`>` or `<`).
- **Value:** The comparison threshold.

For example:

`co2 > 1000`

The API provides the management interface for these rules, while the separate [Sensor Notifications](https://github.com/Markuss-B/sensor-notifications) service evaluates measurements and generates notifications.

Retrieving an individual rule also returns its associated notification history.

## Real-Time Sensor Monitoring

The API uses SignalR and MongoDB Change Streams to deliver new sensor measurements to connected clients.

Instead of continuously polling the database, clients subscribe to a particular sensor and receive updates when new measurements are stored.

### SignalR Hub

**Endpoint:** `/api/sensorhub`

The hub exposes two methods:

| Method | Description |
|---|---|
| `SubscribeToSensor(sensorId)` | Subscribe to measurements from a particular sensor |
| `UnsubscribeFromSensor(sensorId)` | Stop receiving measurements from that sensor |

### Subscription Lifecycle

1. A client connects to the SignalR hub and subscribes to a sensor.
2. `SensorHub` adds the client to the corresponding SignalR group.
3. `GroupTrackingService` records the subscription.
4. `SensorWatcherService` monitors MongoDB for new measurements from the subscribed sensor.
5. When a measurement is stored, it is forwarded to the corresponding SignalR group.
6. When clients unsubscribe, the subscription count is updated.
7. Once no clients remain subscribed to a sensor, the watcher for that sensor is stopped.

### Subscription Tracking

SignalR groups enable broadcasting messages to selected clients but do not directly expose their membership counts.

To address this, the API implements a separate `GroupTrackingService` that tracks connected clients and their sensor subscriptions.

This allows the system to determine when a sensor no longer has active subscribers.

### Database Change Streams

`SensorWatcherService` uses MongoDB Change Streams to observe newly inserted sensor measurements.

New measurements are forwarded to the corresponding SignalR group, allowing the Angular dashboard to update its charts without refreshing the page.

Monitoring is managed according to active subscriptions, avoiding unnecessary database watching for sensors that nobody is viewing.

## Configuration

The application uses ASP.NET Core configuration through `appsettings.json` or environment variables.

### MongoDB Settings

| Setting | Description |
|---|---|
| `MongoDbSettings__ConnectionString` | MongoDB connection string |
| `MongoDbSettings__DatabaseName` | Database name |

### Development Configuration

The application enables Swagger/OpenAPI documentation in the Development environment.

The current CORS configuration allows development frontend origins at:

- `http://localhost`
- `http://localhost:4200`

These settings can be adjusted in `Program.cs` for other deployment environments.

## Running Locally with Docker

The repository includes a Docker Compose configuration in `.build/compose.yaml`.

### Prerequisites

- Docker with Docker Compose
- A running MongoDB instance configured as a replica set
- The shared `sensor-network` Docker network

The local configuration expects MongoDB to be available through the container hostname `sensor-mongo-dev`.

You can start MongoDB using the Docker setup provided in the [Sensor Consumer](https://github.com/Markuss-B/sensor-consumer) repository.

### Start the Web API

If the shared Docker network does not already exist, create it:

```bash
docker network create sensor-network
```

From the `.build` directory, run:

```bash
docker compose up -d --build
```

The API is exposed at:

`http://localhost:5000`

### API Documentation

When running in the Development environment, Swagger UI is available at:

`http://localhost:5000/swagger`

### View Logs

```bash
docker logs -f sensor-webapi-dev
```

### Stop the Application

```bash
docker compose down
```

## Related Components

| Component | Responsibility |
|---|---|
| [Sensor Consumer](https://github.com/Markuss-B/sensor-consumer) | Receives and processes MQTT sensor measurements and metadata, storing them in MongoDB |
| [Sensor Web API](https://github.com/Markuss-B/sensor-webapi) | Provides REST endpoints for sensor and notification management and real-time updates through SignalR |
| [Sensor Dashboard](https://github.com/Markuss-B/sensor-dashboard) | Angular web application for viewing measurements, managing sensors, and configuring notifications |
| [Sensor Notifications](https://github.com/Markuss-B/sensor-notifications) | Evaluates measurement-based notification rules and tracks active and resolved violations |
| [Sensor Courier](https://github.com/Markuss-B/sensor-courier) | Exports measurements from MongoDB to SQL Server or MySQL |
