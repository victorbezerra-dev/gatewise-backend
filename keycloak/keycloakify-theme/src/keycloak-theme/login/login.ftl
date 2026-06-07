<!doctype html>
<html lang="${locale.currentLanguageTag!'pt-BR'}" class="gatewise-login-shell">
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>${msg("loginTitle")}</title>
    <link rel="icon" href="${url.resourcesCommonPath}/img/favicon.ico" />
    <link rel="stylesheet" href="${url.resourcesPath}/css/login.css?v=20260607-no-back-arrow-hover" />
    <style>
      @media (max-width: 1200px) {
        .gw-page { grid-template-columns: 1fr !important; }
        .gw-hero { display: none !important; }
        .gw-brand, .gw-stats { display: none !important; }
        .gw-auth {
          display: flex !important;
          flex-direction: column !important;
          align-items: center !important;
          justify-content: center !important;
          gap: 1.15rem !important;
        }
        .gw-mobile-brand {
          display: flex !important;
          flex-direction: column !important;
          align-items: center !important;
          text-align: center !important;
          width: min(100%, 480px) !important;
          color: #fff !important;
        }
        .gw-mobile-brand-copy h1 {
          color: #fff !important;
          font-size: clamp(2rem, 3.8vw, 2.9rem) !important;
        }
        .gw-mobile-brand-copy p { color: rgba(255, 255, 255, 0.72) !important; }
      }

      @media (max-width: 520px) {
        .gw-page {
          min-height: 100vh !important;
          overflow-x: hidden !important;
          overflow-y: auto !important;
        }

        .gw-auth {
          min-height: 100vh !important;
          min-width: 0 !important;
          align-items: center !important;
          justify-content: center !important;
          padding-top: 2rem !important;
          padding-bottom: 2rem !important;
          gap: 1.25rem !important;
        }

        .gw-mobile-brand {
          display: flex !important;
          flex-direction: column !important;
          align-items: center !important;
          text-align: center !important;
          margin-bottom: 0 !important;
        }

        .gw-mobile-brand-copy h1 {
          font-size: clamp(2rem, 3.8vw, 2.9rem) !important;
        }

        .gw-card {
          min-width: 0 !important;
          margin-left: auto !important;
          margin-right: auto !important;
        }
      }

      @media (max-width: 360px) {
        .gw-auth {
          padding-left: 0.7rem !important;
          padding-right: 0.7rem !important;
        }
      }
    </style>
  </head>
  <body class="gw-body">
    <main class="gw-page">
      <section class="gw-hero" aria-label="GateWise">
        <div class="gw-orb gw-orb-one"></div>
        <div class="gw-orb gw-orb-two"></div>

        <div class="gw-brand">
          <img
            src="${url.resourcesPath}/img/logo.png?v=20260605-logo-trimmed"
            alt="GateWise"
            style="display: block; width: 128px; height: 128px; max-width: 100%; object-fit: contain; filter: drop-shadow(0 22px 46px rgba(0, 0, 0, 0.46));"
          />
          <div class="gw-brand-copy">
            <span class="gw-kicker">${msg("gatewiseLoginEyebrow")}</span>
            <h1>${msg("gatewiseLoginHeroTitle")}</h1>
            <p>${msg("gatewiseLoginHeroSubtitle")}</p>
          </div>
        </div>

        <div class="gw-stats" aria-hidden="true">
          <div><strong>24/7</strong><span>${msg("gatewiseFeatureSecure")}</span></div>
          <div><strong>SSO</strong><span>${msg("gatewiseFeatureFast")}</span></div>
          <div><strong>ID</strong><span>${msg("gatewiseFeatureIntegrated")}</span></div>
        </div>
      </section>

      <section class="gw-auth" aria-label="${msg("loginTitle")}">
        <div class="gw-mobile-brand" aria-hidden="true">
          <img
            src="${url.resourcesPath}/img/logo.png?v=20260605-logo-trimmed"
            alt="GateWise"
            style="display: block; width: 140px; height: auto; max-width: 62vw; margin: 0 auto; object-fit: contain; filter: drop-shadow(0 18px 34px rgba(0, 0, 0, 0.42));"
          />
          <div class="gw-mobile-brand-copy">
            <span class="gw-kicker">${msg("gatewiseLoginEyebrow")}</span>
            <h1>${msg("gatewiseLoginHeroTitle")}</h1>
            <p>${msg("gatewiseLoginHeroSubtitle")}</p>
          </div>
        </div>
   

        <div class="gw-card">
          <div class="gw-card-topline"></div>

          <header class="gw-heading">
            <span class="gw-kicker">${msg("gatewiseLoginAccess")}</span>
            <h2>${msg("gatewiseLoginTitle")}</h2>
            <p>${msg("gatewiseLoginSubtitle")}</p>
          </header>

          <#if message?has_content && (message.type != 'warning' || !isAppInitiatedAction??)>
            <div class="gw-alert gw-alert-${message.type}" role="alert">
              <span></span>
              <p>${kcSanitize(message.summary)?no_esc}</p>
            </div>
          </#if>

          <#assign gwUsernameValue = (login.username!'')>
          <#if message?has_content && message.type == 'success'>
            <#assign gwUsernameValue = ''>
          </#if>

          <#assign gwClientId = (client.clientId)!''>

          <form id="kc-form-login" class="gw-form" action="${url.loginAction}" method="post" onsubmit="var b=document.getElementById('kc-login');if(b){b.disabled=true;b.classList.add('is-loading');var t=b.querySelector('.gw-submit-text');if(t){t.textContent='${msg("gatewiseSigningIn")}';}}">
            <#if usernameHidden?? && usernameHidden>
              <input type="hidden" id="username" name="username" value="${gwUsernameValue}" />
            <#else>
              <label class="gw-field" for="username">
                <span>
                  <#if !realm.loginWithEmailAllowed>
                    ${msg("username")}
                  <#elseif !realm.registrationEmailAsUsername>
                    ${msg("usernameOrEmail")}
                  <#else>
                    ${msg("email")}
                  </#if>
                </span>
                <div class="gw-input">
                  <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 12a4 4 0 1 0-4-4 4 4 0 0 0 4 4Zm0 2c-4.42 0-8 2.24-8 5v1h16v-1c0-2.76-3.58-5-8-5Z"/></svg>
                  <input id="username" name="username" type="text" value="${gwUsernameValue}" autocomplete="username" inputmode="text" autofocus placeholder="${msg("gatewiseUsernamePlaceholder")}" />
                </div>
              </label>
              <div id="gw-username-hint" class="gw-username-hint" aria-live="polite" role="status"></div>
            </#if>

            <label class="gw-field" for="password">
              <span>${msg("password")}</span>
              <div class="gw-input">
                <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M17 8h-1V6a4 4 0 0 0-8 0v2H7a2 2 0 0 0-2 2v9a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2v-9a2 2 0 0 0-2-2Zm-7-2a2 2 0 0 1 4 0v2h-4Zm3 10.73V18h-2v-1.27a2 2 0 1 1 2 0Z"/></svg>
                <input id="password" name="password" type="password" autocomplete="current-password" placeholder="${msg("gatewisePasswordPlaceholder")}" />
                <button class="gw-toggle" type="button" aria-label="${msg("gatewiseShowPassword")}" onclick="const input=document.getElementById('password'); input.type=input.type==='password'?'text':'password'; this.classList.toggle('active');">
                  <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 5c5.5 0 9.5 5 10 7-.5 2-4.5 7-10 7S2.5 14 2 12c.5-2 4.5-7 10-7Zm0 3a4 4 0 1 0 0 8 4 4 0 0 0 0-8Zm0 2a2 2 0 1 1 0 4 2 2 0 0 1 0-4Z"/></svg>
                </button>
              </div>
            </label>

            <div class="gw-options">
              <#if realm.rememberMe && gwClientId != 'gatewise-app' && !(usernameHidden?? && usernameHidden)>
                <label class="gw-check" for="rememberMe">
                  <input id="rememberMe" name="rememberMe" type="checkbox" <#if login.rememberMe??>checked</#if> />
                  <span>${msg("rememberMe")}</span>
                </label>
              <#else>
                <span></span>
              </#if>

              <#if realm.resetPasswordAllowed>
                <a href="${url.loginResetCredentialsUrl}">${msg("doForgotPassword")}</a>
              </#if>
            </div>

            <#if auth.selectedCredential?has_content>
              <input type="hidden" name="credentialId" value="${auth.selectedCredential}" />
            </#if>

            <button class="gw-submit" name="login" id="kc-login" type="submit">
              <span class="gw-submit-spinner" aria-hidden="true"></span>
              <span class="gw-submit-text">${msg("doLogIn")}</span>
              <svg class="gw-submit-icon" viewBox="0 0 24 24" aria-hidden="true"><path d="M13 5 11.6 6.4 16.2 11H4v2h12.2l-4.6 4.6L13 19l7-7Z"/></svg>
            </button>
          </form>

          <p class="gw-register">${msg("noAccount")} <a class="gw-auth-link" href="${url.registrationUrl}">${msg("doRegister")}</a></p>
        </div>
      </section>
    </main>
  <script>
    (function () {
      var input = document.getElementById('username');
      var hint  = document.getElementById('gw-username-hint');
      if (!input || !hint) return;

      var registrationUrl = '${url.registrationUrl}';

      function updateHint() {
        var val = input.value.trim();
        if (!val) { hint.innerHTML = ''; hint.className = 'gw-username-hint'; return; }

        if (/^\d+$/.test(val)) {
          hint.innerHTML = 'Acesso para usu&aacute;rios cadastrados.';
          hint.className = 'gw-username-hint gw-username-hint-registered';
        } else if (val.indexOf('@') !== -1) {
          var registerLink = registrationUrl
            ? ' Sem conta? <a href="' + registrationUrl + '">${msg("doRegister")}</a>'
            : '';
          hint.innerHTML = 'Acesso para usu&aacute;rios externos.' + registerLink;
          hint.className = 'gw-username-hint gw-username-hint-external';
        } else {
          hint.innerHTML = '';
          hint.className = 'gw-username-hint';
        }
      }

      input.addEventListener('input', updateHint);
      updateHint();
    })();
  </script>
  </body>
</html>
