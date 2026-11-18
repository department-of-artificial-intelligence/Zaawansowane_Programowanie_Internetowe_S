import { defineConfig } from 'vite';

let error1Requests = 0;

function sendJson(response, statusCode, body) {
    response.statusCode = statusCode;
    response.setHeader('Content-Type', 'application/json; charset=utf-8');
    response.end(JSON.stringify(body));
}

function labServer() {
    return {
        name: 'rxjs-lab05-server',
        configureServer(server) {
            server.middlewares.use((request, response, next) => {
                const pathname = new URL(request.url ?? '/', 'http://localhost').pathname;

                if (pathname === '/proceed') {
                    if (request.method !== 'POST') {
                        response.setHeader('Allow', 'POST');
                        response.statusCode = 405;
                        response.end();
                        return;
                    }

                    const chunks = [];
                    request.on('data', (chunk) => chunks.push(Buffer.from(chunk)));
                    request.on('end', () => {
                        let body;
                        try {
                            body = JSON.parse(Buffer.concat(chunks).toString('utf8'));
                        } catch {
                            sendJson(response, 400, { error: 'Nieprawidłowy JSON' });
                            return;
                        }

                        sendJson(response, 200, body);
                    });
                    request.on('error', () => {
                        if (!response.writableEnded) {
                            sendJson(response, 400, { error: 'Nie udało się odczytać żądania' });
                        }
                    });
                    return;
                }

                if (pathname === '/error') {
                    response.statusCode = 404;
                    response.end();
                    return;
                }

                if (pathname === '/error1') {
                    error1Requests += 1;
                    const statusCode = error1Requests % 3 === 0 ? 200 : 500;
                    sendJson(response, statusCode, { attempt: error1Requests });
                    return;
                }

                next();
            });
        }
    };
}

export default defineConfig({
    plugins: [labServer()],
    server: {
        host: 'localhost',
        port: 9000,
        strictPort: true
    }
});
