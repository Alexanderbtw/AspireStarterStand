import { registerInstrumentations } from '@opentelemetry/instrumentation';
import { Resource } from "@opentelemetry/resources";
import { SEMRESATTRS_SERVICE_NAME } from "@opentelemetry/semantic-conventions";
import { BatchSpanProcessor, TracerConfig, WebTracerProvider } from "@opentelemetry/sdk-trace-web";
import { OTLPTraceExporter } from "@opentelemetry/exporter-trace-otlp-proto";
import { ZoneContextManager } from "@opentelemetry/context-zone";
import { getWebAutoInstrumentations } from "@opentelemetry/auto-instrumentations-web";

export default function addTracing(url?: string) {
    if (!url) {
        return;
    }

    const collectorOptions = {
        url, 
    };

    const providerConfig: TracerConfig = {
        resource: new Resource({
        [SEMRESATTRS_SERVICE_NAME]: 'react-frontend',
        }),
    };

    const provider = new WebTracerProvider(providerConfig);

    provider.addSpanProcessor(
        new BatchSpanProcessor(new OTLPTraceExporter(collectorOptions)),
    );

    provider.register({
        contextManager: new ZoneContextManager(),
    });

    registerInstrumentations({
        instrumentations: [getWebAutoInstrumentations()],
    });
}
