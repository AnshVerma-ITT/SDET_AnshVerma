import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpServer;

import java.io.IOException;
import java.io.InputStream;
import java.net.InetSocketAddress;
import java.nio.charset.StandardCharsets;
import java.util.concurrent.Executors;
import java.util.concurrent.atomic.AtomicInteger;

public class MockApiServer {

    private static final AtomicInteger ID_COUNTER = new AtomicInteger(1000);

    public static void main(String[] args) throws Exception {

        HttpServer server = HttpServer.create(
                new InetSocketAddress("localhost", 8080),
                0
        );

        server.createContext("/countries/v5", MockApiServer::handleCountries);
        server.createContext("/collections", MockApiServer::handleCollections);

        server.setExecutor(Executors.newFixedThreadPool(200));

        server.start();

        System.out.println("==========================================");
        System.out.println("Local Mock API Server Started");
        System.out.println("Base URL: http://localhost:8080");
        System.out.println("Press Ctrl+C to stop the server.");
        System.out.println("==========================================");
    }

    private static void handleCountries(HttpExchange exchange) throws IOException {

        if (!exchange.getRequestMethod().equalsIgnoreCase("GET")) {
            sendResponse(exchange, 405, "{\"error\":\"Method Not Allowed\"}");
            return;
        }

        // Small artificial delay to make performance measurements realistic.
        try {
    Thread.sleep(50);
} catch (InterruptedException e) {
    Thread.currentThread().interrupt();
}

        String response = """
                {
                  "data": {
                    "objects": [
                      {
                        "cca2": "IN",
                        "name": {
                          "common": "India"
                        }
                      }
                    ],
                    "meta": {
                      "total": 1,
                      "count": 1
                    }
                  }
                }
                """;

        sendResponse(exchange, 200, response);
    }

    private static void handleCollections(HttpExchange exchange) throws IOException {

        String method = exchange.getRequestMethod();
        String path = exchange.getRequestURI().getPath();

        // POST /collections/jmeter-test/objects
        if (method.equalsIgnoreCase("POST")
                && path.equals("/collections/jmeter-test/objects")) {

            try {
    Thread.sleep(80);
} catch (InterruptedException e) {
    Thread.currentThread().interrupt();
}

            // Read request body.
            InputStream inputStream = exchange.getRequestBody();
            String requestBody = new String(
                    inputStream.readAllBytes(),
                    StandardCharsets.UTF_8
            );

            int id = ID_COUNTER.incrementAndGet();

            String response = """
                    {
                      "id": "%d",
                      "name": "JMeter Mock Object",
                      "data": {
                        "description": "Mock object created for JMeter performance testing",
                        "requestTimestamp": "%d"
                      }
                    }
                    """.formatted(id, System.currentTimeMillis());

            sendResponse(exchange, 200, response);
            return;
        }

        // GET /collections/jmeter-test/objects/{id}
        if (method.equalsIgnoreCase("GET")
                && path.startsWith("/collections/jmeter-test/objects/")) {

            try {
    Thread.sleep(40);
} catch (InterruptedException e) {
    Thread.currentThread().interrupt();
}

            String id = path.substring(
                    "/collections/jmeter-test/objects/".length()
            );

            String response = """
                    {
                      "id": "%s",
                      "name": "JMeter Mock Object",
                      "data": {
                        "description": "Mock object created for JMeter performance testing"
                      }
                    }
                    """.formatted(id);

            sendResponse(exchange, 200, response);
            return;
        }

        sendResponse(exchange, 404, "{\"error\":\"Not Found\"}");
    }

    private static void sendResponse(
            HttpExchange exchange,
            int statusCode,
            String response
    ) throws IOException {

        byte[] responseBytes = response.getBytes(StandardCharsets.UTF_8);

        exchange.getResponseHeaders().set(
                "Content-Type",
                "application/json"
        );

        exchange.sendResponseHeaders(
                statusCode,
                responseBytes.length
        );

        exchange.getResponseBody().write(responseBytes);
        exchange.getResponseBody().close();
    }
}