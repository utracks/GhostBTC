# GhostBTC-C#  (Alpha Preview)

** Linux-only, privacy-focused Bitcoin wallet in C#**  
* Work in Progress - Not Production Ready *

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![Build Status](https://img.shields.io/badge/build-alpha-red)
![Platform](https://img.shields.io/badge/platform-linux-only-lightgrey)

## Current Status: Alpha Development

This project is **actively being developed** and has known issues:

### Known Limitations
- **Incomplete Tor integration** (Some connections may bypass Tor)
- **Untested wallet migration** (May lose funds between versions)
- **No GUI implementation** (GTK UI is just a stub)
- **Incomplete test coverage** (Critical paths lack tests)
- **Potential security vulnerabilities** (Not audited)

**DO NOT USE WITH MAINNET FUNDS**  
*Testnet only in current state*

## Features (Planned/Partial)

| Feature | Status | Notes |
|---------|--------|-------|
| Tor Network Routing |  Partial | Some connections may leak |
| HD Wallet (BIP39) |  Working | Testnet only |
| Electrum Server Support |  Partial | Basic connectivity |
| Bitcoin Core RPC |  Not Implemented |  |
| Encrypted Wallet |  Working | AES-256 |
| Coin Control |  Not Implemented |  |
| CLI Interface |  Basic | Missing key features |
| GTK GUI |  Stub Only |  |
