** Project Introduction **

1. Project Scope

	This project aims to create a prototype system for surgical requests, appointment and resourse managment.The system will enable hospitals and clinics to manage surgery appointments and patient records.
	The module in question takes into consideration the legal aspects of the GDPR Regulation (EU) 2016/679, which is a regulation in EU law on data protection and privacy for all individuals within the European Union and the European Economic Area. It also addresses the transfer of personal data outside the EU and EEA areas. The GDPR aims primarily to give control to individuals over their personal data and to simplify the regulatory environment for international business by unifying the regulation within the EU.

2. Project Overview
	
	The system will consist of several modules, the current one is called:
		- Backoffice Web Application

	Overall this module manages:
		- Medical Professionals (doctors, nurses)
		- Patients
		- Operation Types
		- Operation Requests


3. Views

Introduction

We will adopt the combination of two architectural representation models: C4 and 4+1.
4 + 1 View Model

The 4+1 view model is a proposes the description of the system through complementary views, each one with a specific purpose. This way allows the perception of the system from different perspectives, which facilitates the understanding of the system as a whole.

The views are:

    Logical View: It's goal is to answer to business challenges/requirements regarding software aspects.
    Process View: Demonstrates the system's flow and interactions.
    Development View: Regards the software organization in its development environment.
    Physical View: Maps the various software elements to the hardware components, per example, where the software is executed.
    Scenery View: Represents the association between the business processes and the actors that will trigger them.

C4 Model

The C4 model defends the description of software using four levels of abstraction: system context, containers, components and code. Each one adopts a thinner layer of granularity, which allows the more detailed description of the system in smaller parts. We can think of it as a zoom in the system.

The levels are:

    System Context: It's the highest level of abstraction, and it's goal is to demonstrate the system's scope.
    Containers: Describe system containers.
    Components: Describe containers components.
    Code: Describe components code or even more detailed components.

Overview

By combining the two models it is possible to represent the system in various perspectives, each one with different levels of detail.

To represent/modulate, the implemented as well as thought solutions, we will use Unified Modeling Language (UML) as the notation.

	**Level 1**
		![Logical View](.../docs/sprintA/global-artifacts/Level1/Logical View.PNG)


	**Level 2**
		![Logical View](.../docs/sprintA/global-artifacts/Level2/Logical View.PNG)


	**Level 3**
		![Logical View](.../docs/sprintA/global-artifacts/Level3/Logical View.PNG)


	**Generics**

		![Generic PUT](.../docs/sprintA/global-artifacts/Generics/Generic_PUT.puml)

		![Generic POST](.../docs/sprintA/global-artifacts/Generics/Generic_POST.puml)

		![Generic DELETE](.../docs/sprintA/global-artifacts/Generics/Generic_DELETE.puml)

		![Generic GET](.../docs/sprintA/global-artifacts/Generics/Generic_GET.puml)



	