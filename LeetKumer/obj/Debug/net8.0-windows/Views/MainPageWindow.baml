   M S B A M L   `   `   `  ����D  @LeetKumer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:LeetKumer.ViewModelsLeetKumer.ViewModels  5   	   $    LeetKumer.Views.MainPageWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   1vm"clr-namespace:LeetKumer.ViewModels     5   	     � Title$  BookCatalogWindow��5   	   $	��700��6#   $
��1150��60   .��5      � ���#f5f5f5 � ColumnDefinitions 5   
   �� 5      $��20 �6   �� 5	      $��* �6   �� 5
      $��20 �6    � RowDefinitions 5   
   �� 5      $��10 �6   �� 5      $��40 �6   �� 5      �� 5      $��20 �6   .+�5   
   �� ���#4CAF50 �� Orientation$ 
Horizontal=�6*   $��1��6C   $��1��6P   .+�5      �� ���#FFFFFF$��20(�6)   $��Bookman Old Style)�67   $��Center=�6V   $	��295��6q   $��34��6}   $��BookCatalog��6�   �� 5      $	��199��6   $��24��6?   $
��10,0q�6K   $��5q�6Y   ��6"   �� *SearchQuery+6Y   �� 5      $
��🔍��6<   ���#FFFFFF���#4CAF50$��34��6s   $��34��6~   ��6   �� *StartSearchCommand+6~   �� 5      $%��Открыть карточку��6=   ���#FFFFFF���#4CAF50$	��120��6�   $��34��6�   $
��10,0q�6�   ��6   �� *OpenCardBookCommand+6�   �� 5      $!��Добавить книгу��6B   ���#FFFFFF���#4CAF50$	��120��6�   $��34��6�   $
��10,0q�6�   ��6   �� *OpenWindowAddBookCommand+6�   �� 5      $��Удалить книгу��6;   ���#FFFFFF���#4CAF50$	��120��6}   $��34��6�   $
��10,0q�6�   ��6   �� *RemoveBookCommand+6�   �� 5      $	��150��6   ���#FFFFFF
���   �$��1q�6s   ���#4CAF50$	��Top=�6�    �� SelectedItem 6#   �� *SelectedFilter+.@�5      �� $!��Убрать фильтры��6   �� 5      $��По названию��6   �� 5      $��По автору��6   �� 5       $��По жанру��6   �� 5!      $"��По году выпуска��6    �� Text$! Сортировать по��5"   >   5#      5$      � 5'   
   $��2��6   $��1��6   .+�5)      &   System.Windows.Controls.DataGrid  $��2��5*      $	��500��5+      $	��Top=�5,         AutoGenerateColumns$ False��5-      $��0, 0, 0, 10q�5/         CanUserAddRows$ False��50      ��5)      �� *	BooksList+ 5.      �� *SelectedBook+   Columns 51   "   0  *System.Windows.Controls.DataGridTextColumn  52      ,  &System.Windows.Controls.DataGridColumn	   Header$	 Название��6)   
   Width5  /System.Windows.Controls.DataGridLengthConverter$
 2* 6U   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6;   �� *Title+6U     53      $	 
Автор��6)   $
 2* 6\    68   �� *Author.FullName+6\     54      $
	 ISBN��6)   $
 1* 6P    67   �� *ISBN+6P     55      $#	 Дата публикации��6)   $
 1* 6�    6B   �� *YearOfManufacture+ �� StringFormat$ {0:MM.dd.yyyy}��6�     56      $	 Жанр��6)   $
 1* 6V    67   �� *
Genre.Name+6V   58      �� 5:      $	��←��6   $��Bottom=�6I   ���#FFFFFF���#4CAF50$��30��6�   $��30��6�   $��20(�6�   $��Right=�6�   $��5,0,55,5q�6�   ��6!   �� *PreviousPageCommand+6�   �� 5;      $��
Export CSV��6   $��Bottom=�6O   ���#FFFFFF���#4CAF50$��30��6�   $	��120��6�   $��14(�6�   $
��Left=�6�   $��5,0,0,5q�6�   ��6*   �� *ExportCsvCommand+6�   �� 5<      $��Export Excel��6   $��Bottom=�6S   ���#FFFFFF���#4CAF50$��30��6�   $	��120��6�   $��14(�6�   $
��Left=�6�   $��	130,0,0,5q�6�   ��6,   �� *ExportExcelCommand+6�   �� 5=      $��
Import CSV��6   $��Bottom=�6O   ���#FFFFFF���#4CAF50$��30��6�   $	��120��6�   $��14(�6�   $
��Left=�6�   $��	255,0,0,5q�6�   ��6*   �� *ImportCsvCommand+6�   �� 5>      $��Import Excel��6   $��Bottom=�6S   ���#FFFFFF���#4CAF50$��30��6�   $	��120��6�   $��14(�6�   $
��Left=�6�   $��	380,0,0,5q�6�   ��6,   �� *ImportExcelCommand+6�   �� 5?      $��
Export PDF��6   $��Bottom=�6O   ���#FFFFFF���#4CAF50$��30��6�   $	��120��6�   $��14(�6�   $
��Left=�6�   $��	505,0,0,5q�6�   ��6*   �� *ExportPdfCommand+6�   �� 5@      $	��→��6   $��Bottom=�6E   ���#FFFFFF���#4CAF50$��30��6�   $��30��6�   $��20(�6�   $��Right=�6�   $��0,0,5,5q�6�   ��6!   �� *NextPageCommand+6�   5A      5B      5C      