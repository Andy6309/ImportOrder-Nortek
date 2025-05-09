




Import Order - Nortek

Creation Date: 03/07/2025

VERSIONS
 
Date:		Versions:		Author:		Changes:
 
03/07/2025	1.0		AM		Original Description


Introduction:

This document is intended to describe the use and functionality of the ImportOrder.exe created for Nortek.
This application is intended to assist with tedious tasks and improve efficiency of programmers. 

The ImportOrder.exe will do the following:
1.	Import existing order file
2.	Autotool dxf files and follow specific instructions for designated parts
3.	Convert the order file to follow unique ID’s.
4.	Update existing .cp files with unique ID’s.
5.	Import the order to NCExpress.

File locations:

The ImportOrder.exe has several files associated with it. These should all be placed in the following directory.
C:\Prima Power\ncexpress\YOUR_MACHINE_NAME\bin
Here is a list of files needed:

 
![image](https://github.com/user-attachments/assets/c0ba970f-a8f7-48c8-bab8-410d4fbaafbc)




You should also be using the updated Nortek.ink file for production label creation. This will ensure we are gathering all the correct information from the Production_Label info in the order file. 

Date should be:

# Ink Jet device configuration file

HEAD_CONFIGURATION 1
DEVICE_TYPE 2
INDEX_OF_DEFAULT_TEMPLATE 0
CLAMP_PROTECTION 4.24 4.24 2.84

#           ID    #lines   HeadPosition  FeedRate  SizeX  SizeY
# -------------------------------------------------------------------

TEMPLATE    1      9           1           393.7       4     1
Injection Time,<Production_label>
Injection Time,<Production_label>
Job Number,<Production_label>
Dimension XY,<Production_label>
Materials,<Production_label>
Weight,<Production_label>
Part Number,<Production_label>
Part Number Matrix,<Production_label>
New Part Number Matrix,<Production_label>
FORCE_PLACE_LABEL 1


Once all these files are in place, you are ready to use the ImportOrder.exe 

NOTE:
If you ever wish to use NCX Order function as before, simply remove the ImportOrder files from the C:\Prima Power\SGE8_NORTEK\bin location into some backup directory. Be sure to use the Old Nortek.ink file (this is saved in a backup folder).














ImportOrder.exe Overview:

 
![image](https://github.com/user-attachments/assets/12c48f40-0aa2-4e19-a08b-85cd0e8a3ab3)



1: Order and AutoTool

•	This button allows the user to browse for an already existing order file. 
•	Once order file is selected, the user then is prompted to create a new folder for storing .cp files for the desired project.
•	Once folder is created/selected, the application will begin Autotooling the files.

NOTES FOR SPECIAL AUTOTOOLING:

•	All parts with prefix of CFD and CBI get a 1” perimeter added to them and tooled with internal micro joins.
•	All parts that are LESS than 3.151” in width and BETWEEN 28.5 – 45” length will be forced Autotooled at 90 degrees for better nesting.
•	All parts that are LESS than 3.151” in width and LESS than 28.5” length and GREATER than 45” will get the 1” permitter added and tooled with internal micro joints.
•	Please be sure you have good Autotooling parameters for using external micro joints on perimeter parts. If not, you may see incorrect tooling.










2: Open Existing Order

This function was added to allow users to skip the autotooling function of Button 1 and directly open an order file that has previously created .cp files 
If no file is selected application will use top level PartDir set from NCExpress environment.


3: Convert File

This button will take the original order file and convert the following:


•	PartName gets transformed based on the quantity of parts ordered and assigned a unique identifier.
o	For example:
Orignal:
PartName	Quantity
12345	               2

New:
PartName	Quantity
12345-0	1
12345-1	1

•	Additionally, we append the Production_Label information to include this newly modified PartName to the last instance of empty () 


4: Make New CP Files

This button will ensure that a new .cp file is created for each updated PartName with unique identified. We will read the original .cp file and copy the tooling for the new .cp file. This ensures each unique identified part is identical to the original. 









5:  Import Order

This will import the converted order if the previous steps are completed. Application will close automatically upon completed order importation.
If you wish to import an existing order Press Button 2 followed by Button 5. 


6:  Exit

This button will simply close the application. 




Step By Step:

Press Order button form Orders Database of NCX. This will trigger Import Order – Norek application to start:

 



![image](https://github.com/user-attachments/assets/aedba35d-44ff-4f01-8fa2-6c5def146f4c)






This is after Order and Autotool button has been pressed, Order file selected, and new folder created.
Application shows Order file selected, and which parts are processing in progress bar.

 ![image](https://github.com/user-attachments/assets/8ddb8665-2eb6-43b4-abec-a582721a85e6)
![image](https://github.com/user-attachments/assets/5b348a6f-7ff5-4129-85ed-93cbaa0c87fe)

 

Once files are made, you will receive complete prompt:

 


![image](https://github.com/user-attachments/assets/951c6e19-5a53-49ff-a8c3-52a2797bb452)









You will proceed to Convert the file:

 
![image](https://github.com/user-attachments/assets/02cd1da2-c7a0-41b5-baba-b4d3766533ec)


After OK , you will make new CP Files:

 ![image](https://github.com/user-attachments/assets/161290e2-d74c-4e2b-ace2-107a04db1152)


Progess bar will indicate which file is being created.





Press “Ok” to the pop-up indicating completion.

 ![image](https://github.com/user-attachments/assets/4dda33f3-5f67-4cdf-beb8-2e97e96fdab2)


Finally, press IMPORT ORDER button.

 







![image](https://github.com/user-attachments/assets/8eddb3af-cd16-4c0f-9b8d-518d1b872ccb)







Order file will now be present in Orders Database of NCX:

 ![image](https://github.com/user-attachments/assets/0ed71762-dfac-47c6-9235-de321aece005)


Proceed with nesting as normal.
