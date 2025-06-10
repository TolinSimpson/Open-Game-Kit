# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Unity Package Manager support via package.json
- OpenUPM compatibility
- Comprehensive README.md documentation with collapsible feature sections

### Changed
- Improved project structure for Unity package distribution

## [0.1.0] - 2024-01-01

### Added
- **Action System**
  - ActionModule base class with delay support and event handling
  - ActionSequencer component for executing action sequences
  - ActionSequence for managing collections of actions
  - 15+ action categories including Animation, Audio, UI, Physics, and more
  - Action flow control (Continue, Stop, Skip, etc.)

- **Variable System**
  - Support for 8 variable types (String, Int, Float, Double, Vector2, Vector3, Vector4, Quaternion)
  - VariableTracker component for persistent variable management
  - Automatic initialization and reset functionality
  - Variable manipulation through actions

- **Spline System**
  - Support for 6 spline types (Bezier, Hermite, Catmull-Rom, B-Spline, Linear, Raw)
  - Arc-length parameterization for even spacing
  - Local/world space support
  - Path equalization for consistent movement speeds

- **Core Components**
  - AppManager for application lifecycle management
  - ActivationEvents for object state changes
  - PersistentSettings for cross-session data

- **Input System**
  - UserInput and PointerEvents components
  - UI input components (ButtonToggle, GroupToggle, SizeToggle)
  - Cross-platform input support

- **Custom Attributes**
  - SRAttribute for polymorphic serialization
  - MessageAttribute for inspector feedback
  - Enhanced inspector workflow tools

- **Editor Tools**
  - Custom inspectors and property drawers
  - Serialization utilities
  - Editor decorations and workflow enhancements

- **Third-Party Integrations**
  - AI Navigation Package integration
  - PuppetMaster physics integration

- **Logic Operations**
  - Probability systems and mathematical utilities
  - Comparison operations and boolean logic

### Technical
- MIT License
- Unity 2022.3+ compatibility
- Comprehensive namespace organization (OGK)
- Performance-optimized implementations 