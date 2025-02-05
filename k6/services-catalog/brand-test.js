import http from "k6/http";
import { sleep } from "k6";
import { textSummary } from "https://jslib.k6.io/k6-summary/0.0.1/index.js";
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";

const BASE_URL = "http://localhost:5186/api/v1.0/brand/7092769e-c28e-4865-811d-88c918129260";

export const options = {
  stages: [
    { duration: "30s", target: 20 },
    { duration: "1m", target: 20 },
    { duration: "30s", target: 0 },
    { duration: "30s", target: 60 },
  ],
  thresholds: {
    http_req_failed: ['rate<0.01'],
    http_req_duration: ["p(95)<50"],
  },
};

export function handleSummary(data) {
  return {
    "report.html": htmlReport(data),
    stdout: textSummary(data, { indent: " ", enableColors: true }),
  };
}

export default function () {
  http.get(BASE_URL);
  sleep(1);
}