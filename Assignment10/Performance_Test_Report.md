# JMeter API Performance Test Report

## 1. Objective

The objective of this performance test is to evaluate the performance,
response time, throughput, and stability of the API under different
load conditions using Apache JMeter.

The tests measure response time, throughput, error percentage, and
response-time percentiles under different workload conditions.

## 2. APIs Tested

### GET Countries

GET /countries/v5?q={language}

### POST Create Object

POST /collections/jmeter-test/objects

### GET Created Object

GET /collections/jmeter-test/objects/{createdObjectId}

## 3. Test Scenarios

| Scenario | Threads | Ramp-up | Duration |
|---|---:|---:|---:|
| Light | 10 | 10 sec | 60 sec |
| Moderate | 50 | 30 sec | 60 sec |
| Heavy | 100 | 60 sec | 60 sec |
| Stress | 150 | 30 sec | 60 sec |
| Spike | 100 | 1 sec | 30 sec |
| Soak | 20 | 20 sec | 300 sec |
| Volume | 50 | 30 sec | 120 sec |

## 4. Results

| Scenario | Threads | Samples | Error % | Avg (ms) | Median (ms) | P90 (ms) | P95 (ms) | P99 (ms) | Throughput |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Light | 10 | 176 | 0% | 72.65 | 65.00 | 98.00 | 100.35 | 164.90 | 0.07 |
| Moderate | 50 | 713 | 0% | 70.15 | 64.00 | 97.00 | 98.00 | 111.00 | 0.28 |
| Heavy | 100 | 944 | 0% | 68.41 | 63.00 | 95.00 | 97.00 | 101.10 | 0.36 |
| Stress | 150 | 2134 | 0% | 67.70 | 64.00 | 95.00 | 97.00 | 106.65 | 0.81 |
| Spike | 100 | 803 | 0% | 91.44 | 80.00 | 147.00 | 194.00 | 214.96 | 0.30 |
| Soak | 20 | 1880 | 0% | 68.09 | 64.00 | 96.00 | 97.00 | 98.00 | 0.63 |
| Volume | 50 | 1686 | 0% | 67.90 | 64.00 | 96.00 | 97.00 | 98.00 | 0.55 |

## 5. Performance Analysis

### Light Load

The Light Load test used 10 concurrent threads with a 10-second
ramp-up and 60-second duration.

A total of 176 samples were recorded with an error rate of 0%.
The average response time was 72.65 ms and the median response time
was 65 ms.

The P90, P95 and P99 response times were 98 ms, 100.35 ms and
164.90 ms respectively.

The throughput was 0.07 requests per second.

This test provides a baseline for comparing the behavior of the system
under higher loads.

### Moderate Load

The Moderate Load test used 50 concurrent threads with a 30-second
ramp-up and 60-second duration.

A total of 713 samples were recorded with an error rate of 0%.
The average response time decreased to 70.15 ms and the median was
64 ms.

The P90, P95 and P99 values were 97 ms, 98 ms and 111 ms respectively.

Throughput increased to 0.28 requests per second compared with the
Light Load test.

The results indicate that the system continued to process the increased
workload successfully without errors or significant response-time
degradation.

### Heavy Load

The Heavy Load test used 100 concurrent threads with a 60-second
ramp-up and 60-second duration.

A total of 944 samples were recorded and the error rate remained at 0%.

The average response time was 68.41 ms and the median was 63 ms.
The P90, P95 and P99 response times were 95 ms, 97 ms and 101.10 ms.

Throughput increased to 0.36 requests per second.

The response-time values remained stable compared with the Light and
Moderate tests, indicating that the local test environment handled the
higher concurrent workload successfully.

### Stress Test

The Stress Test used the highest configured workload of 150 concurrent
threads with a 30-second ramp-up and 60-second duration.

A total of 2,134 samples were recorded with an error rate of 0%.

The average response time was 67.70 ms and the median was 64 ms.
The P90, P95 and P99 response times were 95 ms, 97 ms and 106.65 ms.

Throughput increased to 0.81 requests per second.

The system continued to process requests successfully under the highest
configured load without recorded errors. The response-time metrics also
remained relatively stable.

Therefore, no clear performance degradation was observed in this test
environment at the configured stress level.

### Spike Test

The Spike Test used 100 concurrent threads with a very short
1-second ramp-up and a 30-second duration.

A total of 803 samples were recorded with an error rate of 0%.

The average response time increased to 91.44 ms and the median increased
to 80 ms.

The P90, P95 and P99 response times increased to 147 ms, 194 ms and
214.96 ms respectively.

Throughput was 0.30 requests per second.

Compared with the other scenarios, the Spike Test produced higher
response-time percentiles. This indicates that the sudden increase in
concurrent users had a greater effect on response-time distribution.

However, the 0% error rate shows that all recorded requests were still
processed successfully.

### Soak Test

The Soak Test used 20 concurrent threads with a 20-second ramp-up and
a duration of 300 seconds.

A total of 1,880 samples were recorded with an error rate of 0%.

The average response time was 68.09 ms and the median was 64 ms.
The P90, P95 and P99 response times were 96 ms, 97 ms and 98 ms.

Throughput was 0.63 requests per second.

The response-time values remained stable during the longer test
duration, and no errors were recorded.

Based on these results, no obvious degradation was observed during the
configured soak period.

### Volume Test

The Volume Test used 50 concurrent threads with a 30-second ramp-up
and a duration of 120 seconds.

A total of 1,686 samples were recorded with an error rate of 0%.

The average response time was 67.90 ms and the median was 64 ms.
The P90, P95 and P99 response times were 96 ms, 97 ms and 98 ms.

Throughput was 0.55 requests per second.

The system maintained stable response times while processing a larger
number of requests over the longer test duration.

## 6. Overall Findings

### Response Time

The average response time remained relatively stable across most load
scenarios, ranging from 67.70 ms to 72.65 ms.

The Spike Test was the main exception, with an average response time
of 91.44 ms.

### Throughput

Throughput increased as the workload and test duration increased.

The highest observed throughput was 0.81 requests per second during the
Stress Test.

### Error Rate

All seven test scenarios recorded an error rate of 0%.

This indicates that no failed requests were recorded during the
performance tests.

### Performance Degradation

No significant performance degradation was observed during the Light,
Moderate, Heavy, Stress, Soak or Volume tests.

The Spike Test showed higher response-time percentiles, indicating that
a sudden increase in traffic had a greater impact on response time.

### Stability

The system remained stable throughout the configured tests.

The Soak Test maintained an average response time of 68.09 ms and a
0% error rate over 300 seconds, with P99 response time of 98 ms.

### Maximum Observed Load

The highest configured load was the Stress Test with:

- 150 concurrent threads
- 30-second ramp-up
- 60-second duration
- 2,134 samples
- 0% error rate
- 67.70 ms average response time
- 0.81 requests/second throughput

## 7. CPU and Memory Analysis

CPU and memory utilization were monitored using Windows Task Manager
during the performance testing.

The observations were used to understand the resource utilization of
the local test environment while increasing the workload.

The CPU and memory measurements represent the local machine and should
not be considered measurements of a production server.

| Test | CPU Utilization | Memory Utilization |
|---|---:|---:|
| Stress | Approximately 14%–91% during the Stress test | 79% |

### Observation

CPU and memory utilization were monitored during the performance tests
and compared with response time, throughput and error percentage.

The performance tests were executed against the local mock API
environment, therefore the resource measurements represent the local
test environment.

## 8. Conclusion

The JMeter performance tests successfully evaluated the APIs under
Light, Moderate, Heavy, Stress, Spike, Soak and Volume workloads.

All seven scenarios recorded a 0% error rate, indicating that all
recorded requests were successfully processed.

The average response time remained relatively stable across most
scenarios, with values between 67.70 ms and 72.65 ms. The Spike Test
showed comparatively higher response times, particularly at the P95
and P99 levels, demonstrating the effect of a sudden increase in
traffic.

The Stress Test handled the highest configured load of 150 concurrent
threads with 2,134 recorded samples, 0% errors, an average response
time of 67.70 ms and throughput of 0.81 requests per second.

The Soak Test also showed stable behavior during the 300-second test
duration.

Overall, the results indicate stable behavior of the API in the local
performance testing environment under the configured workloads.