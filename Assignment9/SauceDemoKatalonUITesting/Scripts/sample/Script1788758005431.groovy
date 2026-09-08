import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject
import static com.kms.katalon.core.testobject.ObjectRepository.findWindowsObject
import com.kms.katalon.core.checkpoint.Checkpoint as Checkpoint
import com.kms.katalon.core.cucumber.keyword.CucumberBuiltinKeywords as CucumberKW
import com.kms.katalon.core.llm.keyword.LlmKeywords as LLM
import com.kms.katalon.core.mobile.keyword.MobileBuiltInKeywords as Mobile
import com.kms.katalon.core.model.FailureHandling as FailureHandling
import com.kms.katalon.core.testcase.TestCase as TestCase
import com.kms.katalon.core.testdata.TestData as TestData
import com.kms.katalon.core.testng.keyword.TestNGBuiltinKeywords as TestNGKW
import com.kms.katalon.core.testobject.TestObject as TestObject
import com.kms.katalon.core.webservice.keyword.WSBuiltInKeywords as WS
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI
import com.kms.katalon.core.windows.keyword.WindowsBuiltinKeywords as Windows
import internal.GlobalVariable as GlobalVariable
import org.openqa.selenium.Keys as Keys

WebUI.openBrowser(null)

WebUI.navigateToUrl('https://www.saucedemo.com/')

WebUI.click(findTestObject('Login/input_Username'))

WebUI.rightClick(findTestObject('Login/input_Username'))

WebUI.assertElementPresent(findTestObject('Login/input_Username'), 0)

WebUI.setText(findTestObject('Login/input_Username'), 'standard_user')

WebUI.setEncryptedText(findTestObject('Login/input_Password'), 'qcu24s4901FyWDTwXGr6XA==')

WebUI.rightClick(findTestObject('Login/input_login-button'))

WebUI.assertElementPresent(findTestObject('Login/input_login-button'), 0)

WebUI.click(findTestObject('Login/input_login-button'))

WebUI.click(findTestObject('Inventory/div_ProductsName (A to Z)Name (A to Z)Name (Z to'))

WebUI.rightClick(findTestObject('Common/span_Products'))

WebUI.assertElementText(findTestObject('Common/span_Products'), 'Products', 0)

WebUI.click(findTestObject('Inventory/div_inventory_container'))

WebUI.click(findTestObject('Inventory/div_Sauce Labs Backpack'))

WebUI.rightClick(findTestObject('Inventory/div_Sauce Labs Backpack'))

WebUI.assertElementText(findTestObject('Inventory/div_Sauce Labs Backpack'), 'Sauce Labs Backpack', 0)

