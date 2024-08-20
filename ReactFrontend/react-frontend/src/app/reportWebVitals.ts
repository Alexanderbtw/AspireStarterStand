import { Metric } from 'web-vitals';

const reportWebVitals = (onReport: (metric: Metric) => void) => {
    import('web-vitals').then(({ onCLS, onFCP, onINP, onTTFB, onLCP }) => {
        onCLS(onReport);
        onFCP(onReport);
        onINP(onReport);
        onTTFB(onReport);
        onLCP(onReport);
      });
  };
  
export default reportWebVitals;