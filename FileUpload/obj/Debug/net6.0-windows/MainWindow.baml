   M S B A M L   `   `   `  ����E  AFileUpload, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:FileUpload
FileUpload  5   	       FileUpload.MainWindow   5   	   W SPresentationCore, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:FileUpload     5   	     � Title$  UploadMe��5   	   $	��450��6   $	��800��6'   .��5	      �  � RowDefinitions 5
   
   �� 5      $
��1.5* �6   �� 5      $
��8.5* �6   .+�5   
   �� $��0��6   
�������$��24(�60   ��5      �� 5      .��5      �  � Color$ #2c3e50��6#    � Offset$ 0Q�63   � 5      $ #bdc3c7��6#   $ 1Q�63   5      .��5       	Upload Me5      � 5   
   $��1��6    � ColumnDefinitions 5      �� 5      $	��200 �6#   �� 5      .+�5      �� $��0��6   .+�5      �� $��40��6   .��6%   	All Files60   �� 5      $��40��6   .��6%   
Categories61   5      � 5       $��1��6    5!      �� 5"      $��9* �6$   �� 5#      $��1* �6$   .+�5%      � $��0��6   .+�5&      �� -   5         ��Name$ 
FileViewer��5&      $��Stretch=�63   $	��Top=�6Q   $��Disabled=�6i   ��5'      �� 5(       �� 
TargetType" ��6$   .�5)   "   ��  �� Property � �� �� Value$ Stretch=�6O   5*      ��5,      �� 5-      .R�5.   "   A� 	 A� Columns$	 5��6.   $��Stretch=�6:   5/      ��51      �� 52      .R�53   "   �� $��70��6-   
 �� Orientation$
 Vertical=�6:   $��Stretch=�6Q   $��Stretch=�6m   .+�54   &   �� $��50��6,   $��Center=�67   $	��Top=�6r   ��6T   �� *	ImageData+6r   �� 55   &   $
��Wrap=�60    �� TextAlignment$ Right=�6D   $��Center=�6Z   $��Bottom=�6�   ��6w   �� *
FileName+6�    � ContextMenu 56   &   �� 57   *   .@�58   .   m� -   5        $��Download��58   7   
 � Tag 6^   �� *
FilePath+ m� Icon 59   2   �� 5:   6   $��assets/download_icon.png��6<   5<   /   m� 5=   .   -   5        $��Delete��5=   7    6Z   �� *FileID+ 5>   2   �� 5?   6   $��assets/delete_icon.png��6<   5A   /   5B   +   5D   #   5E      5G      5H      � 5I      $��1��6   .+�5J      � -   5         �Name$
 pBar��5J   F    � Minimum$ 0Q�6"    � Maximum$	 100Q�6.    � Value$ 0Q�6<   $y�Hidden=�6T   �� 5K      -   5         ��Name$ BrowseButton��5K      $��Browse a file��63   $��Center=�6K   $��Center=�5L      68   5M      5N      5O      5P      5Q      