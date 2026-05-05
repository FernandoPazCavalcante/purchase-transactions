# Smoke + e2e tests against running API.
# Run: hurl --test --variable host=http://localhost:8080 tests/api.hurl

# ---------- health ----------
GET {{host}}/api/health
HTTP 200

# ---------- POST happy path ----------
POST {{host}}/api/transactions
Content-Type: application/json
{
  "description": "lunch",
  "date": "2026-04-01T00:00:00Z",
  "amountUsd": 12.50
}
HTTP 201
[Captures]
tx_id: jsonpath "$.id"
[Asserts]
header "Location" contains "/api/transactions/"
jsonpath "$.description" == "lunch"
jsonpath "$.amountUsd" == 12.50

# ---------- GET by id ----------
GET {{host}}/api/transactions/{{tx_id}}
HTTP 200
[Asserts]
jsonpath "$.id" == "{{tx_id}}"
jsonpath "$.description" == "lunch"

# ---------- GET 404 ----------
GET {{host}}/api/transactions/00000000-0000-0000-0000-000000000000
HTTP 404

# ---------- GET convert happy path ----------
GET {{host}}/api/transactions/{{tx_id}}/convert?currency=Brazil-Real
HTTP 200
[Asserts]
jsonpath "$.id" == "{{tx_id}}"
jsonpath "$.amountUsd" == 12.50
jsonpath "$.exchangeRate" > 0
jsonpath "$.convertedAmount" > 0

# ---------- convert: missing tx -> 404 ----------
GET {{host}}/api/transactions/00000000-0000-0000-0000-000000000000/convert?currency=Brazil-Real
HTTP 404

# ---------- convert: unknown currency -> 422 ----------
GET {{host}}/api/transactions/{{tx_id}}/convert?currency=Atlantis-Doubloon
HTTP 422
[Asserts]
jsonpath "$.error" contains "the purchase cannot be converted to Atlantis-Doubloon"

# ---------- validation: empty description ----------
POST {{host}}/api/transactions
Content-Type: application/json
{
  "description": "",
  "date": "2026-04-01T00:00:00Z",
  "amountUsd": 10.00
}
HTTP 400
[Asserts]
jsonpath "$.errors.Description" exists

# ---------- validation: description >50 chars ----------
POST {{host}}/api/transactions
Content-Type: application/json
{
  "description": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "date": "2026-04-01T00:00:00Z",
  "amountUsd": 10.00
}
HTTP 400
[Asserts]
jsonpath "$.errors.Description" exists

# ---------- validation: future date ----------
POST {{host}}/api/transactions
Content-Type: application/json
{
  "description": "future",
  "date": "2099-01-01T00:00:00Z",
  "amountUsd": 10.00
}
HTTP 400
[Asserts]
jsonpath "$.errors.Date" exists

# ---------- validation: negative amount ----------
POST {{host}}/api/transactions
Content-Type: application/json
{
  "description": "neg",
  "date": "2026-04-01T00:00:00Z",
  "amountUsd": -1.00
}
HTTP 400
[Asserts]
jsonpath "$.errors.AmountUsd" exists

# ---------- validation: amount with >2 decimals ----------
POST {{host}}/api/transactions
Content-Type: application/json
{
  "description": "precision",
  "date": "2026-04-01T00:00:00Z",
  "amountUsd": 1.234
}
HTTP 400
[Asserts]
jsonpath "$.errors.AmountUsd" exists
