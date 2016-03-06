using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Vendors;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Stores;


namespace Nop.Services.Messages
{
    public partial class WorkflowMessageService
    {

        public virtual int SendVendorEmailValidationMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate("Vendor.EmailValidationMessage", store.Id);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            _messageTokenProvider.AddVendorTokens(tokens, customer);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var toEmail = customer.Email;
            var toName = customer.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName);
        }

        public virtual int SendProductInquery(Product product,Vendor vendor,string from,string phone, string email,string msg, int languageId)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate("Product.Inqure", store.Id);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);


            tokens.Add(new Token("Product.ProductUrl", store.Url + "/Product/" + product.Name));
            tokens.Add(new Token("Product.ProductName", product.Name));
            tokens.Add(new Token("From", from));
            tokens.Add(new Token("Phone", phone));
            tokens.Add(new Token("Email", email));
            tokens.Add(new Token("Msg", msg));
          


            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var toEmail = vendor.Email;
            var toName = vendor.Name;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName);
        }
    }
}
