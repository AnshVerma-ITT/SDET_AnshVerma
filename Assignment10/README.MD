# API Performance Testing - JMeter Project

## Project Details

| Field | Value |
| --- | --- |
| Project Name | API Performance Testing - JMeter |
| Developed By | Ansh Verma |
| Application Type | API Performance Testing Project |
| Performance Testing Tool | Apache JMeter |
| APIs Tested | REST Countries API and RESTful API |
| Test Data | CSV Parameterization |
| Scripting | Groovy |
| Reporting | JMeter HTML Dashboard |
| Load Scenarios | Light, Moderate, Heavy, Stress, Spike, Soak, Volume |
| Mock Server | Java HTTP Server |

## Project Description

The API Performance Testing project is a JMeter-based performance testing project developed to validate API functionality and measure API performance under different load conditions.

The project demonstrates performance testing concepts including load generation, response-time measurement, throughput analysis, percentile analysis, assertions, parameterization, dynamic data generation, API chaining, correlation, timers, and HTML reporting.

The test plan contains API requests for retrieving country information and creating and retrieving objects.

A local Java mock API server is also included for safe and repeatable performance testing without consuming the request quota of the external APIs.

## Assignment Implementation Summary

- **API Performance Testing:** API endpoints are tested under multiple load conditions.
- **JMeter Test Plan:** A complete JMeter test plan is created containing Thread Groups, HTTP Request Samplers, Assertions, Timers, Listeners, CSV Data Set Config, and configuration elements.
- **CSV Parameterization:** Test data is read dynamically from `TestData.csv`.
- **Dynamic Data:** Product names are dynamically generated using the product name from CSV and the current timestamp.
- **API Chaining:** The object ID returned from the POST request is extracted and passed to the following GET request.
- **Correlation:** The `createdObjectId` JMeter variable is used to correlate the POST response with the subsequent GET request.
- **Response Assertions:** Response code, response duration, and JSON response content are validated.
- **Think Time:** A Constant Timer is used to simulate a 3-second user think time between API operations.
- **Configurable Load:** Thread count, ramp-up time, duration, loop count, startup delay, and scenario name can be passed as JMeter properties.
- **Multiple Load Scenarios:** Light, Moderate, Heavy, Stress, Spike, Soak, and Volume scenarios are executed.
- **HTML Reporting:** JMeter HTML dashboards are generated for each performance scenario.
- **Performance Metrics:** Average response time, median, percentiles, throughput, sample count, and error percentage are analyzed.
- **CPU and Memory Monitoring:** System CPU and memory utilization are observed during the Stress test using Windows Task Manager.
- **HTTPS Test Script Recorder:** A separate HTTPS Test Script Recorder demonstration is included.

## Features Implemented

### 1. API Testing

- GET country information
- POST object creation
- GET created object
- HTTP response validation
- JSON response validation
- Response duration validation
- API request chaining

### 2. CSV Parameterization

Test data is stored in:

```text
TestData/TestData.csv
```

Example:

```csv
searchTerm,productName,productDescription
Canada,iPhone 15,Apple smartphone
Germany,Samsung Galaxy,Android smartphone
India,MacBook Air,Apple laptop
France,Dell XPS,Laptop computer
```

JMeter reads the values dynamically using the CSV Data Set Config element.

The following variables are used:

```text
${searchTerm}
${productName}
${productDescription}
```

### 3. Dynamic Test Data

A JSR223 PreProcessor using Groovy generates dynamic product names.

```groovy
def productName = vars.get("productName")
def timestamp = System.currentTimeMillis().toString()
def dynamicProductName = "${productName}-${timestamp}"

vars.put("requestTimestamp", timestamp)
vars.put("dynamicProductName", dynamicProductName)
```

This prevents every POST request from using exactly the same product name.

### 4. API Correlation

The POST response returns a dynamically generated object ID.

The ID is extracted using a JSON/JMESPath extractor:

```text
Variable Name:
createdObjectId

JMESPath Expression:
id
```

The extracted value is then used in the following GET request:

```text
/collections/jmeter-test/objects/${createdObjectId}
```

This demonstrates API chaining and correlation in JMeter.

### 5. Assertions

The test plan contains assertions for:

- HTTP response code
- Maximum response duration
- JSON response content
- Correlated object ID

GET Countries:

```text
Expected Response Code:
200

JSON JMESPath:
data.objects[0].cca2

Expected Value:
IN
```

POST Create Object:

```text
Expected Response Code:
200
```

GET Created Object:

```text
Expected Response Code:
200
```

The maximum configured response time is:

```text
3000 ms
```

### 6. Think Time

A Constant Timer is configured between the country GET request and object creation request.

```text
Think Time:
3000 ms
```

This simulates a user spending some time between two API operations.

## Performance Test Scenarios

| Scenario | Threads | Ramp-up | Duration |
| --- | ---: | ---: | ---: |
| Light | 10 | 10 sec | 60 sec |
| Moderate | 50 | 30 sec | 60 sec |
| Heavy | 100 | 60 sec | 60 sec |
| Stress | 150 | 30 sec | 60 sec |
| Spike | 100 | 1 sec | 30 sec |
| Soak | 20 | 20 sec | 300 sec |
| Volume | 50 | 30 sec | 120 sec |

### Scenario Purpose

**Light Load**

Represents a small number of concurrent users and establishes a basic performance baseline.

**Moderate Load**

Represents a normal increase in concurrent users.

**Heavy Load**

Tests the system with a higher number of concurrent users.

**Stress Test**

Uses the highest configured load to observe system behavior under heavy pressure.

**Spike Test**

Rapidly increases the number of users using a 1-second ramp-up period to observe the effect of a sudden traffic increase.

**Soak Test**

Runs for an extended period to check performance stability over time.

**Volume Test**

Uses a larger amount of API activity to observe system behavior when processing a higher transaction volume.

## Local Performance Test Environment

Performance execution was performed against a local Java mock API server.

The local server runs on:

```text
http://localhost:8080
```

The following endpoints are provided:

```text
GET
/countries/v5?q=${searchTerm}
```

```text
POST
/collections/jmeter-test/objects
```

```text
GET
/collections/jmeter-test/objects/${createdObjectId}
```

The local mock server provides predictable responses and avoids consuming the request quota of the external API during load testing.

## Mock API Server

The mock API server is implemented using Java's built-in HTTP server.

Project location:

```text
MockApiServer/
├── MockApiServer.java
└── MockApiServer.class
```

Compile the server using:

```powershell
& "C:\Program Files\Eclipse Adoptium\jdk-21.0.12.101-hotspot\bin\javac.exe" -d . .\MockApiServer.java
```

Start the server using:

```powershell
& "C:\Program Files\Eclipse Adoptium\jdk-21.0.12.101-hotspot\bin\java.exe" -cp . MockApiServer
```

The server displays:

```text
==========================================
Local Mock API Server Started
Base URL: http://localhost:8080
Press Ctrl+C to stop the server.
==========================================
```

## JMeter Configuration

The performance environment uses the following configuration variables:

| Variable | Value |
| --- | --- |
| `PERF_PROTOCOL` | `http` |
| `PERF_HOST` | `localhost` |
| `PERF_PORT` | `8080` |

The Thread Group also supports configurable JMeter properties:

| Property | Purpose |
| --- | --- |
| `threads` | Number of virtual users |
| `rampup` | User ramp-up time |
| `duration` | Test duration |
| `loops` | Loop count |
| `delay` | Startup delay |
| `scenario` | Performance scenario name |

Example Thread Group configuration:

```text
Threads:
${__P(threads,10)}

Ramp-up:
${__P(rampup,10)}

Loop Count:
${__P(loops,-1)}

Duration:
${__P(duration,60)}

Startup Delay:
${__P(delay,0)}

Scenario:
${__P(scenario,LOCAL)}
```

## JMeter Test Plan Structure

```text
API Performance Testing - RestCountries and Restful API
│
├── CSV Test Data
├── Environment Variables
├── Common HTTP Headers
│
└── API Performance Tests
    │
    ├── API Requests
    │   │
    │   └── Complete API Transaction
    │       │
    │       ├── Debug Sampler
    │       │
    │       ├── GET Countries
    │       │   ├── GET - Response Code Assertion
    │       │   ├── GET - Duration Assertion
    │       │   └── GET - JSON Content Assertion
    │       │
    │       ├── User Think Time
    │       │
    │       ├── POST Create Object
    │       │   ├── POST - Response Code Assertion
    │       │   ├── POST - Duration Assertion
    │       │   ├── POST - JSON Content Assertion
    │       │   ├── POST - Extract Object ID
    │       │   └── POST - Generate Dynamic Data
    │       │
    │       └── GET Created Object
    │           ├── GET Created Object - Response Code Assertion
    │           ├── GET Created Object - Duration Assertion
    │           ├── GET Created Object - JSON Content Assertion
    │           └── GET Created Object - ID Correlation Assertion
    │
    ├── View Results Tree
    ├── View Results in Table
    └── Summary Report
```

> The Debug Sampler is used only for debugging and should be disabled during actual performance execution because it adds additional samples to the test results.

## Running JMeter Tests

JMeter can be executed from the command line using non-GUI mode.

JMeter executable:

```text
C:\apache-jmeter-5.6.3\bin\jmeter.bat
```

Example Light Load execution:

```powershell
& "C:\apache-jmeter-5.6.3\bin\jmeter.bat" -n -t ".\TestPlan\API Performance Test.jmx" -Jthreads=10 -Jrampup=10 -Jduration=60 -Jscenario=LIGHT -l ".\Results\Light\Light_Load.jtl"
```

Example Moderate Load execution:

```powershell
& "C:\apache-jmeter-5.6.3\bin\jmeter.bat" -n -t ".\TestPlan\API Performance Test.jmx" -Jthreads=50 -Jrampup=30 -Jduration=60 -Jscenario=MODERATE -l ".\Results\Moderate\Moderate_Load.jtl"
```

Example Heavy Load execution:

```powershell
& "C:\apache-jmeter-5.6.3\bin\jmeter.bat" -n -t ".\TestPlan\API Performance Test.jmx" -Jthreads=100 -Jrampup=60 -Jduration=60 -Jscenario=HEAVY -l ".\Results\Heavy\Heavy_Load.jtl"
```

Example Stress execution:

```powershell
& "C:\apache-jmeter-5.6.3\bin\jmeter.bat" -n -t ".\TestPlan\API Performance Test.jmx" -Jthreads=150 -Jrampup=30 -Jduration=60 -Jscenario=STRESS -l ".\Results\Stress\Stress_Load.jtl"
```

Example Spike execution:

```powershell
& "C:\apache-jmeter-5.6.3\bin\jmeter.bat" -n -t ".\TestPlan\API Performance Test.jmx" -Jthreads=100 -Jrampup=1 -Jduration=30 -Jscenario=SPIKE -l ".\Results\Spike\Spike_Load.jtl"
```

Example Soak execution:

```powershell
& "C:\apache-jmeter-5.6.3\bin\jmeter.bat" -n -t ".\TestPlan\API Performance Test.jmx" -Jthreads=20 -Jrampup=20 -Jduration=300 -Jscenario=SOAK -l ".\Results\Soak\Soak_Load.jtl"
```

Example Volume execution:

```powershell
& "C:\apache-jmeter-5.6.3\bin\jmeter.bat" -n -t ".\TestPlan\API Performance Test.jmx" -Jthreads=50 -Jrampup=30 -Jduration=120 -Jscenario=VOLUME -l ".\Results\Volume\Volume_Load.jtl"
```

## HTML Performance Reports

JMeter HTML dashboards are generated from the `.jtl` result files.

Example:

```powershell
& "C:\apache-jmeter-5.6.3\bin\jmeter.bat" -g ".\Results\Stress\Stress_Load.jtl" -o ".\Reports\Stress"
```

The same process is used for the other scenarios.

The generated HTML reports contain performance information such as:

- Response time
- Throughput
- Error percentage
- Response time distribution
- Percentiles
- Active threads
- Requests per second
- Statistics
- Response time over time

## Performance Metrics

The following metrics are analyzed:

| Metric | Description |
| --- | --- |
| Average | Average response time of requests |
| Median | Middle response time |
| P90 | 90% of requests completed within this time |
| P95 | 95% of requests completed within this time |
| P99 | 99% of requests completed within this time |
| Throughput | Number of requests processed per unit of time |
| Error % | Percentage of failed requests |
| Samples | Total number of requests executed |

## Performance Test Results

| Scenario | Threads | Samples | Error % | Avg (ms) | Median (ms) | P90 (ms) | P95 (ms) | P99 (ms) | Throughput |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Light | 10 | 176 | 0% | 72.65 | 65.00 | 98.00 | 100.35 | 164.90 | 0.07 |
| Moderate | 50 | 713 | 0% | 70.15 | 64.00 | 97.00 | 98.00 | 111.00 | 0.28 |
| Heavy | 100 | 944 | 0% | 68.41 | 63.00 | 95.00 | 97.00 | 101.10 | 0.36 |
| Stress | 150 | 2134 | 0% | 67.70 | 64.00 | 95.00 | 97.00 | 106.65 | 0.81 |
| Spike | 100 | 803 | 0% | 91.44 | 80.00 | 147.00 | 194.00 | 214.96 | 0.30 |
| Soak | 20 | 1880 | 0% | 68.09 | 64.00 | 96.00 | 97.00 | 98.00 | 0.63 |
| Volume | 50 | 1686 | 0% | 67.90 | 64.00 | 96.00 | 97.00 | 98.00 | 0.55 |

## Performance Analysis

### Light Load

The Light scenario used 10 threads with a 10-second ramp-up and 60-second duration.

The test executed 176 samples with a 0% error rate.

The average response time was 72.65 ms and the median response time was 65 ms. P90 was 98 ms, P95 was 100.35 ms, and P99 was 164.90 ms.

The system showed stable behavior under the light workload.

### Moderate Load

The Moderate scenario used 50 threads with a 30-second ramp-up and 60-second duration.

A total of 713 samples were executed with 0% errors.

The average response time was 70.15 ms and the median was 64 ms. P90 was 97 ms, P95 was 98 ms, and P99 was 111 ms.

The response time remained stable compared with the Light scenario.

### Heavy Load

The Heavy scenario used 100 threads with a 60-second ramp-up and 60-second duration.

The test executed 944 samples with 0% errors.

The average response time was 68.41 ms and the median was 63 ms. P90 was 95 ms, P95 was 97 ms, and P99 was 101.10 ms.

No significant performance degradation was observed.

### Stress Load

The Stress scenario used the highest configured load of 150 threads.

A total of 2134 samples were executed with a 0% error rate.

The average response time was 67.70 ms and the median was 64 ms. P90 was 95 ms, P95 was 97 ms, and P99 was 106.65 ms.

Throughput reached 0.81.

The system remained stable under the highest configured load in the local test environment.

### Spike Load

The Spike scenario used 100 threads with a 1-second ramp-up and 30-second duration.

The test executed 803 samples with 0% errors.

The average response time increased to 91.44 ms.

P90 increased to 147 ms, P95 increased to 194 ms, and P99 reached 214.96 ms.

This scenario showed the highest response-time percentiles, demonstrating the effect of a sudden increase in traffic.

### Soak Load

The Soak scenario used 20 threads for 300 seconds.

A total of 1880 samples were executed with 0% errors.

The average response time was 68.09 ms and the median was 64 ms.

P90 was 96 ms, P95 was 97 ms, and P99 was 98 ms.

The results indicate stable behavior during the longer-duration test.

### Volume Load

The Volume scenario used 50 threads for 120 seconds.

A total of 1686 samples were executed with 0% errors.

The average response time was 67.90 ms and the median was 64 ms.

P90 was 96 ms, P95 was 97 ms, and P99 was 98 ms.

The system remained stable while processing the higher transaction volume.

## Overall Findings

- All seven performance scenarios completed with a **0% error rate**.
- Average response times were generally between approximately **67 ms and 73 ms**.
- The Spike test produced the highest average response time at **91.44 ms**.
- The Spike test also produced the highest P95 and P99 response times.
- The Stress test used the highest configured load of **150 threads**.
- The Stress test executed **2134 samples** with **0% errors**.
- The highest observed throughput was **0.81** during the Stress scenario.
- The Soak test remained stable during the 300-second execution.
- The Volume test also maintained stable response times.
- No significant performance degradation was observed in the local performance testing environment.

## CPU and Memory Analysis

System resource utilization was observed using Windows Task Manager during the Stress test.

| Resource | Observation |
| --- | --- |
| CPU Utilization | Approximately 14% - 91% |
| Memory Utilization | Approximately 79% |

During the Stress test, CPU utilization varied from approximately 14% to 91%, indicating significant CPU activity under the highest configured load.

Memory utilization was approximately 79%, showing relatively high but manageable memory usage during the test.

These values were observed from Windows Task Manager and should be considered approximate system-level observations.

## HTTPS Test Script Recorder

A separate JMeter HTTPS Test Script Recorder demonstration is included.

File:

```text
Recorder/HTTPS_Recorder_Demonstration.jmx
```

The recorder demonstration contains:

```text
HTTPS Test Script Recorder - Demonstration
│
├── HTTP(S) Test Script Recorder
│
└── Recording Thread Group
    └── Recording Controller
```

Recorder configuration includes:

```text
Port:
8888

Target Controller:
Use Recording Controller

Grouping:
Do not group samplers

Capture HTTP Headers:
Enabled

Add Assertions:
Disabled

Regex Matching:
Disabled
```

The recorder can be used to capture HTTP/HTTPS requests and generate JMeter samplers from recorded browser traffic.

## Project Structure

```text
Assignment10
│
├── TestPlan
│   └── API Performance Test.jmx
│
├── TestData
│   └── TestData.csv
│
├── Results
│   ├── Light
│   ├── Moderate
│   ├── Heavy
│   ├── Stress
│   ├── Spike
│   ├── Soak
│   └── Volume
│
├── Reports
│   ├── Light
│   ├── Moderate
│   ├── Heavy
│   ├── Stress
│   ├── Spike
│   ├── Soak
│   └── Volume
│
├── Recorder
│   └── HTTPS_Recorder_Demonstration.jmx
│
├── MockApiServer
│   ├── MockApiServer.java
│   └── MockApiServer.class
│
├── Performance_Test_Report.md
│
└── README.md
```

## Technologies and Concepts Used

- Apache JMeter
- REST API
- HTTP methods
- GET requests
- POST requests
- JSON
- JSON/JMESPath assertions
- Response code assertions
- Duration assertions
- CSV Data Set Config
- Parameterization
- JMeter variables
- JMeter properties
- API chaining
- Correlation
- JSON/JMESPath extraction
- JSR223 PreProcessor
- Groovy
- Constant Timer
- Thread Groups
- HTTP Request Samplers
- Listeners
- View Results Tree
- View Results in Table
- Summary Report
- Non-GUI execution
- JTL result files
- HTML Dashboard
- Response time
- Latency
- Throughput
- Percentiles
- Load testing
- Stress testing
- Spike testing
- Soak testing
- Volume testing
- CPU monitoring
- Memory monitoring
- HTTPS Test Script Recorder
- Java Mock API Server

## Conclusion

This project demonstrates API performance testing using Apache JMeter.

It covers API request creation, CSV parameterization, dynamic test data, assertions, timers, API chaining, correlation, configurable load testing, multiple performance testing scenarios, command-line execution, JTL result generation, HTML performance reports, and system resource monitoring.

The executed tests showed a 0% error rate across all configured scenarios in the local performance environment. Response times remained generally stable under Light, Moderate, Heavy, Stress, Soak, and Volume workloads, while the Spike scenario showed increased response-time percentiles because of the sudden traffic increase.

The project also includes an HTTPS Test Script Recorder demonstration and a local Java mock API server for repeatable performance testing.
