export default {
  '/api': {
    target: process.env['API_URL'] ?? 'http://localhost:5100',
    secure: false,
  },
  '/simulate': {
    target: process.env['TELEMETRY_URL'] ?? 'http://localhost:5200',
    secure: false,
  },
};
