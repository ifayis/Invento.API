# Reports Module

## Overview

The Reports module provides business insights by aggregating operational and financial data.

Reports are optimized using **Dapper** to execute efficient SQL queries for read-heavy workloads.

---

# Available Reports

## Sales Report

Provides:

- Sales History
- Revenue
- Profit
- Customer Information
- Invoice Details

Supports:

- Pagination
- Searching
- Date Range Filtering

---

## Purchase Report

Provides:

- Purchase History
- Supplier Information
- Purchase Totals
- Payment Status

Supports:

- Pagination
- Supplier Filtering
- Date Range Filtering

---

## Inventory Report

Provides:

- Product Stock
- Low Stock
- Critical Stock
- Inventory Value

---

## Cash Flow Report

Displays:

- Income
- Expenses
- Manual Credits
- Manual Debits
- Net Cash Flow

---

## Customer Receivables

Displays:

- Outstanding Customer Balances
- Payments Received
- Remaining Due

---

## Supplier Payables

Displays:

- Outstanding Supplier Balances
- Payments Made
- Remaining Due

---

# Dashboard

The dashboard summarizes business performance.

Metrics include:

- Today's Sales
- Monthly Sales
- Monthly Profit
- Monthly Purchases
- Cash Balance
- Outstanding Receivables
- Outstanding Payables
- Low Stock Products
- Critical Stock Products

---

# Top Products

Displays the highest-performing products based on sales.

---

# Top Customers

Displays customers ranked by total purchase value.

Information includes:

- Customer
- Orders
- Revenue
- Profit
- Recent Purchases

---

# Performance

Reporting queries use:

- Dapper
- Optimized SQL
- Aggregations
- SQL Indexes
- Pagination

This reduces execution time for analytical queries.

---

# Filtering

Most reports support:

- Search
- Date Range
- Pagination
- Customer Filter
- Supplier Filter

---

# Multi-Tenant Reporting

All reports are filtered by TenantId.

Each tenant only sees its own business data.