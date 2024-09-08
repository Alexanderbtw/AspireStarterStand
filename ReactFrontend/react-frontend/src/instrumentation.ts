import { registerOTel } from "@vercel/otel";

export function register() {
    const serviceName = process.env["OTEL_SERVICE_NAME"];
    registerOTel({
        serviceName
    });
}